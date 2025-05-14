using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventaire : MonoBehaviour
{
    [SerializeField] private Inventaire inv;
    public List<Image> spriteSlots;
    private List<ItemData> allItems;
    public GameObject tooltip;
    public TMP_Text tooltipText;
    public RectTransform tooltipRect;
    public Image inspectionImage;

    private List<Sprite> initialSprites;
    private readonly Dictionary<Image, TMP_Text> stackTexts = new();

    private void Start()
    {
        initialSprites = new List<Sprite>(spriteSlots.Count);
        foreach (Image slot in spriteSlots)
        {
            initialSprites.Add(slot.sprite);
        }

        allItems = inv.GetInventaire();
        UpdateUI();
        HideTooltip();
        inspectionImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
        {
            HideInspectionImage();
        }
    }

    public void UpdateUI()
    {
        List<Sprite> sprites = inv.GetAllSprites();
        Dictionary<Sprite, int> itemCounts = new();

        foreach (Sprite sprite in sprites)
        {
            if (sprite != null)
            {
                itemCounts[sprite] = itemCounts.GetValueOrDefault(sprite, 0) + 1;
            }
        }

        int slotIndex = 0;
        foreach (var (sprite, count) in itemCounts)
        {
            if (slotIndex >= spriteSlots.Count) break;

            Image slot = spriteSlots[slotIndex];
            slot.sprite = sprite;
            slot.color = Color.white;

            int itemIndex = sprites.IndexOf(sprite);
            AddTooltip(slot, itemIndex);
            AddClickEvent(slot, itemIndex);

            TMP_Text stackText = GetOrCreateStackText(slot);
            stackText.enabled = count > 1;
            stackText.text = count > 1 ? count.ToString() : string.Empty;

            slotIndex++;
        }

        for (int i = slotIndex; i < spriteSlots.Count; i++)
        {
            Image slot = spriteSlots[i];
            slot.sprite = null;
            slot.color = Color.white;

            if (stackTexts.TryGetValue(slot, out TMP_Text stackText))
            {
                stackText.text = string.Empty;
                stackText.enabled = false;
            }
        }
    }

    public void AddTooltip(Image slot, int index)
    {
        EventTrigger trigger = slot.gameObject.GetComponent<EventTrigger>() ?? slot.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((_) => ShowTooltip(index, slot));
        trigger.triggers.Add(entryEnter);

        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((_) => HideTooltip());
        trigger.triggers.Add(entryExit);
    }

    public void AddClickEvent(Image slot, int index)
    {
        Button button = slot.gameObject.GetComponent<Button>() ?? slot.gameObject.AddComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => InspectItem(index));
    }

    public void ShowTooltip(int index, Image slot)
    {
        if (index >= allItems.Count) return;

        tooltipText.text = $"{allItems[index].itemName}\n{allItems[index].itemDescription}";
        tooltip.SetActive(true);
        Vector3 slotPosition = slot.transform.position;
        tooltipRect.position = new Vector3(slotPosition.x, slotPosition.y + 100, slotPosition.z);
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }

    public void InspectItem(int index)
    {
        if (index < allItems.Count && allItems[index].type.ToString() == "inspectable")
        {
            inspectionImage.sprite = inv.GetAllSprites()[index];
            inspectionImage.gameObject.SetActive(true);
        }
    }

    public void HideInspectionImage()
    {
        inspectionImage.gameObject.SetActive(false);
    }

    private bool IsPointerOverUIElement()
    {
        PointerEventData eventData = new(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    public int GetUnmodifiedSprite()
    {
        int unmodifiedCount = 0;
        for (int i = 0; i < spriteSlots.Count; i++)
        {
            if (spriteSlots[i].sprite == initialSprites[i])
            {
                unmodifiedCount++;
            }
        }
        return unmodifiedCount;
    }

    private TMP_Text GetOrCreateStackText(Image slot)
    {
        if (stackTexts.TryGetValue(slot, out TMP_Text stackText))
        {
            return stackText;
        }

        GameObject textObj = new("StackText");
        textObj.transform.SetParent(slot.transform, false);
        textObj.transform.localScale = Vector3.one;

        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
        rectTransform.pivot = new Vector2(1, 0);
        rectTransform.anchoredPosition = new Vector2(-5, 5);

        stackText = textObj.AddComponent<TextMeshProUGUI>();
        stackText.fontSize = 24;
        stackText.alignment = TextAlignmentOptions.BottomRight;
        stackText.color = Color.white;
        stackText.raycastTarget = false;

        stackTexts.Add(slot, stackText);
        return stackText;
    }
}