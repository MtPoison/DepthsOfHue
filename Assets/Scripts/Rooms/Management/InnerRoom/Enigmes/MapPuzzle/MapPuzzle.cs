using System.Collections.Generic;
using UnityEngine;

public class MapPuzzle : Enigme
{
    [SerializeField] private GameObject mapCanvas;

    public List<MapLocations> rightOrderList = new List<MapLocations>();
    public List<GameObject> chosenOrderList = new List<GameObject>();
    private List<LineRenderer> listLineRenderer = new List<LineRenderer>();
    [SerializeField] private GameObject canvaEnigme;
    [SerializeField] private PlayEffect playEffect;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioClip success;


    public GameObject prefabLine;
    public static MapPuzzle Instance;

    #region Event
    public delegate void SendSoundEffect(AudioClip _clip);

    public static event SendSoundEffect OnSendSoundEffect;
    
    #endregion
    
    public override void Initialize()
    {
        mapCanvas.SetActive(true);
        canvaEnigme.SetActive(false);


        if (Instance == null)
        {
            Instance = this;
        }
        base.Initialize();
    }
    
    public void AddToChosenList(GameObject newObject)
    {
        Location objLocation = newObject.GetComponent<Location>();
        if (!objLocation || objLocation.isVisited) return;
        chosenOrderList.Add(newObject);
        objLocation.VisitLoc();
        if (chosenOrderList.Count > 1)
        {
            GameObject obj1 = chosenOrderList[^2];
            GameObject obj2 = chosenOrderList[^1];
            Vector3[] pos =
            {
                new Vector3(obj1.transform.position.x,obj1.transform.position.y,obj1.transform.position.z-0.05f),
                new Vector3(obj2.transform.position.x,obj2.transform.position.y,obj1.transform.position.z-0.05f)
            };
            LineRenderer lineRenderer = Instantiate(prefabLine,mapCanvas.transform).GetComponent<LineRenderer>();
            listLineRenderer.Add(lineRenderer);
            lineRenderer.SetPositions(pos);
            lineRenderer.gameObject.SetActive(true);
            if (playEffect && clip) playEffect.PlaySoundEffect(clip);
        }
        CheckOrder();
    }

    public void RemoveLastPosition()
    {
        switch (chosenOrderList.Count)
        {
            case <= 0:
                return;
            case >= 2:
            {
                LineRenderer line = listLineRenderer[^1];
                listLineRenderer.Remove(line);
                Destroy(line);
                break;
            }
        }

        chosenOrderList[^1].GetComponent<Location>().UnvisitLoc();
        chosenOrderList.Remove(chosenOrderList[^1]);
    }

    private void CheckOrder()
    {
        if (rightOrderList.Count != chosenOrderList.Count)
        {
            return;
        }

        for (int i = 0; i < rightOrderList.Count; i++)
        {
            if (rightOrderList[i] != chosenOrderList[i].GetComponent<Location>().locationName)
            {
                DialogueManager.Instance.StartNewDialogue(2, DialogueGroupKey.carteDestin);
                return;
            }
        }

        mapCanvas.SetActive(false);
        canvaEnigme.SetActive(true);
        DialogueManager.Instance.StartNewDialogue(1, DialogueGroupKey.carteDestin);
        Success();
        if (success) OnSendSoundEffect?.Invoke(success);
    }

    public void Quit()
    {
        canvaEnigme.SetActive(true);
        mapCanvas.SetActive(false);


        foreach (var line in listLineRenderer)
        {
            Destroy(line.gameObject);
        }
        listLineRenderer.Clear();

        foreach (var obj in chosenOrderList)
        {
            var loc = obj.GetComponent<Location>();
            if (loc != null)
            {
                loc.UnvisitLoc();
            }
        }
        chosenOrderList.Clear();
    }

}
