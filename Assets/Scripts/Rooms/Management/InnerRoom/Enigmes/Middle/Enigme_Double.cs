using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class Enigme_Doble : Enigme
{
    private GameObject firstSelected;
    private GameObject secondSelected;
    private GameObject firstBulleSelected;
    private GameObject secondBulleSelected;
    private int nbDouble;
    [SerializeField] private List<GameObject> ItemsDouble;
    [SerializeField] private Camera cam;
    [SerializeField] private ImgBackGroundEnigme img;
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject zoneText;
    [SerializeField] private GameObject bubulle;
    [SerializeField] private GestionInputs gestion;
    [SerializeField] private List<Vector3> AllPosition;
    private GameObject foundObject;
    private GameObject bubble;
    private List<GameObject> bulles = new List<GameObject>();
    private float timer = 1f; // 1 seconde
    private bool isTimerRunning = false;

   private void Awake()
    {
        if (!bubulle) return;
        PrepareBulles();
    }

    private void PrepareBulles()
    {
        //Vector3 pos = new Vector3(1000f, 0f, 0f);
        for (int i = 0; i < ItemsDouble.Count; i++)
        {
            GameObject newBulle = Instantiate(bubulle, Vector3.zero, Quaternion.identity);
            SpawnObjects(newBulle);
            newBulle.SetActive(false);
            bulles.Add(newBulle);
        }
        CreateBulle();
    }

    public override void Initialize()
    {
        base.Initialize();
        nbDouble = ItemsDouble.Count / 2;
        foundObject = GameObject.Find("bulle");
        UpdateTexte();
        FramesManager.Instance.LockFrame("Pillar");
        /*Vector3 vector = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 5);

        if (vector != null)
        {
            img.transform.position = vector;
        }*/

    }

    public void ObjectClicked(GameObject obj)
    {
        if (!gestion.enabled) return;
        Bulle bubulle = obj.GetComponent<Bulle>();
        if (bubulle.item == firstSelected) return;

        if (firstSelected == null && firstBulleSelected == null)
        {
            firstSelected = bubulle.item;
            firstBulleSelected = obj;
            bubulle.item.transform.position = obj.transform.position;
            bubulle.item.SetActive(true);
            obj.SetActive(false);
        }
        else
        {
            gestion.enabled = false;
            secondSelected = bubulle.item;
            secondBulleSelected = obj;
            bubulle.item.transform.position = obj.transform.position;
            bubulle.item.SetActive(true);
            obj.SetActive(false);
            
            isTimerRunning = true;
            
        }
    }


    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Debug.Log("Timer termin� !");
                isTimerRunning = false;
                CheckObj();
                timer = 1f;
            }
        }
    }

    public void UpdateTexte()
    {
        text.text = nbDouble.ToString();
    }

    public void CheckObj()
    {
        ItemDouble firstObj = firstSelected.GetComponent<ItemDouble>();
        ItemDouble secondObj = secondSelected.GetComponent<ItemDouble>();
        if(firstObj == null || secondObj == null)
        {
            return;
        }
        if (firstObj.GetId() == secondObj.GetId())
        {
            firstSelected.SetActive(false);
            secondSelected.SetActive(false);
            firstSelected = null;
            secondSelected = null;
            firstBulleSelected = null;
            secondBulleSelected = null;
            nbDouble--;
            if (nbDouble == 4)
            {
                DialogueManager.Instance.StartNewDialogue(2,enigmeDialogKey);
            }

            if (nbDouble == 2)
            {
                DialogueManager.Instance.StartNewDialogue(3,enigmeDialogKey);
            }
            UpdateTexte();
            Success();
            gestion.enabled = true;
        }
        else
        {
            firstSelected.SetActive(false);
            secondSelected.SetActive(false);
            firstBulleSelected.SetActive(true);
            secondBulleSelected.SetActive(true);
            firstBulleSelected = null;
            secondBulleSelected = null;
            firstSelected = null;
            secondSelected = null;
            gestion.enabled = true;

        }
    }

    protected override void Success()
    {
        if(nbDouble == 0 && !isResolved)
        {
            base.Success();
            zoneText.SetActive(false);
            DialogueManager.Instance.StartNewDialogue(1,enigmeDialogKey);
        }
    }


    private void SpawnObjects(GameObject _obj)
    {
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        float minX = bottomLeft.x + 7f;
        float maxX = topRight.x - 7f;
        float minY = bottomLeft.y + 5f;
        float maxY = topRight.y - 5f;

        float minDistance = 3f;


        int id = Random.Range(0, AllPosition.Count);
        _obj.transform.position = AllPosition[id];

        AllPosition.RemoveAt(id);
    }



    public override void CheckItem(GameObject item)
    {
        base.CheckItem(item);
        ObjectClicked(item);
    }

    public void CreateBulle()
    {
        if (ItemsDouble == null || ItemsDouble.Count == 0)
        {
            Debug.LogError("ItemsDouble est null ou vide !");
            return;
        }

        List<GameObject> list = new List<GameObject>(ItemsDouble);
        for (int i = 0; i < bulles.Count; i++)
        {
            if (list.Count == 0)
                break;

            int index = Random.Range(0, list.Count);
            GameObject bulleObj = bulles[i];
            bulleObj.SetActive(true);
            Bulle bulleScript = bulleObj.GetComponent<Bulle>();
            bulleScript.item = list[index];
            list.RemoveAt(index);
        }
    }

}

