using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enigme_Pillar : Enigme
{
    [Header("Prefabs & References")]
    public GameObject pillarPrefab;
    public GameObject textPrefab;
    public GameObject popUp;
    public GestionInputs ray;

    [Header("Data")]
    public List<string> items;
    public List<Material> materials;

    [Header("Scene Objects")]
    public List<GameObject> sceneObjects;
    public float spacing = 2f;

    private GameObject pillarTake = null;
    private List<Pillar> pillars = new List<Pillar>();
    private List<string> OrderEnigme = new List<string>();
    private List<Material> pillarMaterials = new List<Material>();

    public GameObject pillarContainer;

    [SerializeField] ImgBackGroundEnigme img;

    public bool firstPillarClicked = false;
    public GameObject indiceButton;

    public GameObject PillarPrefab
    {
        get => pillarPrefab;
        set => pillarPrefab = value;
    }

    public List<string> Items
    {
        get => items;
        set => items = value;
    }

    public GameObject TextPrefab
    {
        get => textPrefab;
        set => textPrefab = value;
    }

    public GameObject PopUp
    {
        get => popUp;
        set => popUp = value;
    }

    public List<GameObject> SceneObjects
    {
        get => sceneObjects;
        set => sceneObjects = value;
    }

    public float Spacing
    {
        get => spacing;
        set => spacing = value;
    }

    public GestionInputs Ray
    {
        get => ray;
        set => ray = value;
    }

    public List<Pillar> Pillars
    {
        get => pillars;
        set => pillars = value;
    }

    public List<Material> Materials
    {
        get => materials;
        set => materials = value;
    }

    private void Start()
    {
        popUp.SetActive(false);
        RandomPillar();
    }

    public override void Initialize()
    {
        FramesManager.Instance.LockFrame("main_frame");
        FramesManager.Instance.UnlockFrame("Pillar");
        base.Initialize();
        if (popUp == null || textPrefab == null)
        {
            Debug.LogError("popUp ou textPrefab n'est pas assigne dans l'inspecteur.");
            return;
        }
        Vector3 vector = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y,5);
        //img.transform.position = vector;
        popUp.SetActive(false);
        SpawnPillars();
        
    }

    public void UpdatePopup()
    {

        ClearPopup();

        if (pillarTake.GetComponent<Pillar>().IsObj)
        {
            PopUpTake();
        }
        else PopUpItem();
    }

    private void ClearPopup()
    {
        foreach (Transform child in popUp.transform)
        {
            if (child != null) Destroy(child.gameObject);
        }
    }

    private void PopUpTake()
    {
        GameObject newText = Instantiate(textPrefab, popUp.transform);
        if (newText == null) return;

        TextMeshProUGUI textMesh = newText.GetComponent<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = "reprendre";
            textMesh.enabled = true;
            textMesh.ForceMeshUpdate();
            textMesh.tag = "Cube";

            Button textButton = newText.GetComponent<Button>() ?? newText.AddComponent<Button>();
            textButton.onClick.AddListener(OnResumeClicked);
        }

        RectTransform textRect = newText.GetComponent<RectTransform>();
        if (textRect != null) textRect.anchoredPosition = Vector2.zero;

        RectTransform popUpRect = popUp.GetComponent<RectTransform>();
        if (popUpRect != null) popUpRect.sizeDelta = new Vector2(200, 50);
    }

    private void PopUpItem()
    {
        float totalHeight = 0f;
        float maxWidth = 0f;
        float spacingY = 25f;
        float horizontalPadding = 20f;

        for (int i = 0; i < items.Count; i++)
        {
            string item = items[i];
            GameObject newText = Instantiate(textPrefab, popUp.transform);
            if (newText == null)
            {
                Debug.LogError("�chec de l'instanciation de textPrefab.");
                continue;
            }

            TextMeshProUGUI textMesh = newText.GetComponent<TextMeshProUGUI>();
            if (textMesh == null)
            {
                Debug.LogError("TextMeshProUGUI manquant sur textPrefab.");
                Destroy(newText);
                continue;
            }

            textMesh.text = item;
            textMesh.enabled = true;
            textMesh.ForceMeshUpdate();

            totalHeight += textMesh.preferredHeight + spacingY;
            maxWidth = Mathf.Max(maxWidth, textMesh.preferredWidth);

            Button textButton = newText.GetComponent<Button>() ?? newText.AddComponent<Button>();
            textButton.onClick.AddListener(() => OnTextClicked(item));

            RectTransform textRect = newText.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.anchorMin = new Vector2(0.5f, 1f);
                textRect.anchorMax = new Vector2(0.5f, 1f);
                textRect.pivot = new Vector2(0.5f, 0.5f);
                textRect.anchoredPosition = new Vector2(0f, -totalHeight + (textMesh.preferredHeight / 2) + spacingY * i);
            }
        }

        RectTransform popUpRect = popUp.GetComponent<RectTransform>();
        if (popUpRect != null) popUpRect.sizeDelta = new Vector2(maxWidth + horizontalPadding, totalHeight);
        
    }

    private void OnResumeClicked()
    {
        if (pillarTake == null) return;

        items.Add(pillarTake.GetComponent<Pillar>().Objet.name);
        sceneObjects.Add(pillarTake.GetComponent<Pillar>().Objet);
        pillarTake.GetComponent<Pillar>().Objet.transform.position = new Vector3(1000, 1000, 0);

        pillarTake.GetComponent<Pillar>().SetTake();
        pillarTake.GetComponent<Pillar>().Objet = null;

        popUp.SetActive(false);
    }

    private void SpawnPillars()
    {
        if (pillarPrefab == null) return;

        int count = items.Count;
        float offset = (count - 1) * spacing / 2f;

        for (int i = 0; i < count; i++)
        {
            float x = i * spacing - offset;
            Vector3 position = new Vector3(x, -4.15f, 0f);

            Debug.Log("pillar");

            Quaternion rotation = Quaternion.Euler(-90f, 180f, 0f);

            GameObject newPillar = Instantiate(pillarPrefab, pillarContainer.transform.GetChild(i).position, rotation);
            newPillar.transform.localScale = new Vector3(132, 132, 132);
            if (newPillar == null)
            {
                Debug.LogError("�chec de l'instanciation de pillarPrefab.");
                continue;
            }

            Pillar pillarScript = newPillar.GetComponent<Pillar>() ?? newPillar.AddComponent<Pillar>();

            newPillar.GetComponent<MeshRenderer>().material = pillarMaterials[i];
            pillarScript.Popup = popUp;
            pillarScript.Spawner = this;
            pillarScript.Ray = ray;
            pillarScript.ID = OrderEnigme[i];

            pillars.Add(pillarScript);
        }
    }

    private void RandomPillar()
    {
        List<string> itemsCopy = new List<string>(items);
        List<Material> materialsCopy = new List<Material>(materials);

        for (int i = 0; i < items.Count; i++)
        {
            int Index = UnityEngine.Random.Range(0, itemsCopy.Count);

            OrderEnigme.Add(itemsCopy[Index]);
            pillarMaterials.Add(materialsCopy[Index]);

            itemsCopy.RemoveAt(Index);
            materialsCopy.RemoveAt(Index);
        }
    }

    private void OnTextClicked(string itemName)
    {
        List<GameObject> remainingObjects = new List<GameObject>();

        foreach (GameObject obj in sceneObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                remainingObjects.Add(obj);
            }
        }

        GameObject selectedObject = remainingObjects.Find(obj => obj.name == itemName);
        if (selectedObject != null)
        {
            selectedObject.SetActive(true);

            if (pillarTake != null)
            {
                Vector3 pillarTop = pillarTake.GetComponent<Renderer>().bounds.center + new Vector3(0, pillarTake.GetComponent<Renderer>().bounds.extents.y, 0);

                Vector3 newPosition = pillarTop;
                newPosition.y += CalculateDistanceToBottom(selectedObject);
                selectedObject.transform.position = newPosition;
            }

            remainingObjects.Remove(selectedObject);
            items.Remove(itemName);

            UpdatePopup();
            popUp.SetActive(false);

            sceneObjects.Clear();
            sceneObjects.AddRange(remainingObjects);

            pillarTake.GetComponent<Pillar>().SetTake();
            pillarTake.GetComponent<Pillar>().Objet = selectedObject;

            Success();
        }
        else
        {
            Debug.LogWarning($"Objet '{itemName}' non trouv� dans la sc�ne.");
        }
    }

    private float CalculateDistanceToBottom(GameObject obj)
    {
        Bounds bounds;
        if (obj.TryGetComponent(out Renderer renderer))
        {
            bounds = renderer.bounds;
        }
        else if (obj.TryGetComponent(out Collider collider))
        {
            bounds = collider.bounds;
        }
        else return 0f;

        return bounds.extents.y;
    }

    

    protected override void Success()
    {
        print("check");
        foreach (var pillar in pillars)
        {
            if (pillar.Objet == null || pillar.Objet.name != pillar.ID) return;
        }
        
        base.Success();
    }
    public override void CheckItem(GameObject item)
    {
        base.CheckItem(item);
        foreach (var pillar in pillars)
        {
            if (item == pillar.gameObject)
            {
                pillar.OnObjectClicked(item);
                pillarTake = item;
                UpdatePopup();

            }
        }
    }

    public void ShowIndiceButton()
    {
        indiceButton.SetActive(true);
    }
}
