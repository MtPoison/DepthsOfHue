using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


[System.Serializable]
public class PositionData
{
   
    public string roomName;
    public Vector3 worldPosition;
    public Quaternion worldRotation;

    public PositionData(string roomName, Vector3 worldPosition, Quaternion worldRotation)
    {
        this.roomName = roomName;
        this.worldPosition = worldPosition;
        this.worldRotation = worldRotation;
    }
}

public class Enigme_FindObjects : Enigme
{
    public GameObject start;
    public GameObject inGame;


    [SerializeField] int maxRound = 3;
    public int currentRound = 0;

    public TextMeshProUGUI[] text;
    public GameObject panel;


    [SerializeField] private float timeLimit = 60f;
    private float timer; //current time


    [SerializeField] private List<GameObject> objectsUsedInEnigme; // Every object dynamically chose for the enigme

    public List<PositionData> allObjectsPositions = new List<PositionData>();

    private List<GameObject> instantiatedObjects = new List<GameObject>();



    [SerializeField] private int amountObjectsUsed = 3; // How many objects are used for this enigme

    [SerializeField] private AudioClip success;
    
    #region Event

    public delegate void SendSoundEffect(AudioClip _clip);

    public static event SendSoundEffect OnSendSoundEffect;

    #endregion
    
    public override void Initialize()
    {

        start.SetActive(false);
        inGame.SetActive(true);

        CollectAllPositionsFromScene();

        base.Initialize();
        DialogueManager.Instance.StartEnterPuzzleRoom(enigmeDialogKey);
        RoundRoutine();     
    }

    public void RoundRoutine()
    {
        foreach (GameObject obj in objectsInEnigme) {

            obj.transform.localScale = new Vector3(45, 45, 45);

        }

        ClearClones();

        objectsUsedInEnigme = MakeObjectsList();
        ClearText();
        SetObject();
        if(currentRound != 0)
            DialogueManager.Instance.StartNewDialogue(currentRound+1,enigmeDialogKey);
        timer = timeLimit;
        panel.SetActive(true);
        StartTimer();
    }



    private void CollectAllPositionsFromScene()
    {
        GameObject container = GameObject.FindWithTag("PositionContainer");

        if (container == null)
        {
            return;
        }

        allObjectsPositions.Clear();

        foreach (Transform room in container.transform)
        {
            string roomName = room.name;

            foreach (Transform pos in room)
            {

                Vector3 worldPos =  pos.position;
                Quaternion worldRot = pos.rotation;
           

                allObjectsPositions.Add(new PositionData(roomName, worldPos, worldRot));
                var meshRenderer = pos.GetComponent<MeshRenderer>();
                if (meshRenderer != null) meshRenderer.enabled = false;

                var meshFilter = pos.GetComponent<MeshFilter>();
                if (meshRenderer != null)
{
    meshRenderer.enabled = false;
}
            }
        }

        Debug.Log($"Nombre total de positions r�cup�r�es : {allObjectsPositions.Count}");
    }



    void StartTimer()
    {
        if (EnigmaTimerManager.Instance != null)
        {
            EnigmaTimerManager.Instance.ShowTimer();
        }
    }

    /// <summary>
    /// This function chose random items and return a list of these objects. List size depends on amountObjectsUsed variable.
    /// </summary>
    private List<GameObject> MakeObjectsList()
    {
        List<GameObject> list = new List<GameObject>(); //Temporary list
        var temp = new List<GameObject>(objectsInEnigme); // Clone objects in scene (to remove elements)

        int io = 0;

        for (int i = 0; i < amountObjectsUsed && temp.Count > 0; i++) //Choose random items
        {


            int index = Random.Range(0, temp.Count);
            list.Add(temp[index]);
            temp.RemoveAt(index);

            text[io].text = list[io].gameObject.name;

            io++;

        }

        return list;

    }

    private void Update()
    {
        if (isStarted)
        {

            UpdateEnigme(Time.deltaTime);
        }
    }
    public override void UpdateEnigme(float deltaTime)
    {

        if (isResolved) return;


        timer -= deltaTime;

        if (EnigmaTimerManager.Instance != null)
        {
            EnigmaTimerManager.Instance?.UpdateTimerDisplay(timer); //Updates the timer display
        }


        if (timer <= 0f)
        {
            DialogueManager.Instance.StartNewDialogue(4,enigmeDialogKey);
            Fail();
        }
    }

    /// <summary>
    /// This function checks if an item is part of the winning condition item list.
    /// Parameter expects an item.
    /// </summary>
    /// <param name="item"></param>
    public override void CheckItem(GameObject item)
    {
        if (item == null) return;


        if (objectsUsedInEnigme.Contains(item.gameObject))
        {

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].text == item.name)
                {
                    text[i].fontStyle = FontStyles.Strikethrough;

                    objectsUsedInEnigme.Remove(item.gameObject);

                    CheckEndOfRound();


                    break;
                }
            }

            MoveFragment(item ,item.transform.position + new Vector3(-10, 0, 0), new Vector3(0.0f, 0.0f, 0.0f));
            //item.SetActive(false); 
        }
    }

    public void MoveFragment(GameObject item, Vector3 targetPosition, Vector3 finalScale)
    {
        Vector3 start = item.transform.position;
        float duration = 2f;

        // La rotation d'origine de l'objet
        Quaternion originalRotation = item.transform.rotation;

        float radius = 2.5f; // Plus grand = spirale plus large
        int spinCount = 2; // Nombre de tours
        float angle = 0f;

        DOTween.To(() => angle, x => {
            angle = x;
            float t = angle / (360f * spinCount);

            // Interpolation lin�aire de la position de base � finale
            Vector3 center = Vector3.Lerp(start, targetPosition, t);

            // Cr�ation d�un offset circulaire
            float radians = Mathf.Deg2Rad * angle;
            Vector3 offset = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f
            ) * radius * (1 - t);

            // Applique la position finale
            item.transform.position = center + offset;

        }, 360f * spinCount, duration).SetEase(Ease.InOutSine);

        // Scale
        item.transform.DOScale(finalScale, duration).SetEase(Ease.OutBack);

        // Rotation sur lui-m�me
        item.transform.DORotate(new Vector3(360f, 360f, 360f), duration, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutSine)
                     .OnKill(() =>
                     {
                         // Une fois l'animation termin�e, restaurer la rotation d'origine
                         item.transform.rotation = originalRotation;
                     });
    }

    /// <summary>
    /// This function is used to check the end of the current round. It will react accordingy.
    /// </summary>
    private void CheckEndOfRound()
    {
        if (IsRoundEnded())
        {
            if (currentRound < (maxRound - 1))
            {
                
                currentRound++;
                StartCoroutine(Wait(3)); 
                
            }
            else
            {
                panel.SetActive(false);
                DialogueManager.Instance.StartNewDialogue(3,enigmeDialogKey);
                Success(); // End the enigme and invoke succes event
                if (success) OnSendSoundEffect?.Invoke(success);
            }
        }

    }

    /// <summary>
    /// Wait coroutine 
    /// Parameter exepcting a second amount
    /// </summary>
    /// <param name="sec"></param>
    /// <returns></returns>
    IEnumerator Wait(float sec)
    {
        float elapsedTime = 0f;
        float duration = sec;

        while (elapsedTime < duration)
        {
           

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        RoundRoutine();
    }

    /// <summary>
    /// This function returns the state of the round ending. True = ended.
    /// </summary>
    /// <returns></returns>
    private bool IsRoundEnded()
    {
        return (objectsUsedInEnigme.Count == 0);
    }

    private void ClearText()
    {
        for (int i = 0; i < text.Length; i++)
        {
            text[i].fontStyle = default;
        }
    }

    /// <summary>
    /// Reset objetcs position randomly + reset their scale
    /// </summary>
    private void SetObject()
    {
        var availablePositions = new List<PositionData>(allObjectsPositions);
        var objectTempoEnigme = new List<GameObject>(objectsInEnigme);


        foreach (GameObject objprime in objectsUsedInEnigme)
        {
            if (objectTempoEnigme.Contains(objprime))
            {
                objectTempoEnigme.Remove(objprime);
            }

            int index = Random.Range(0, availablePositions.Count);
            PositionData posData = availablePositions[index];

            objprime.transform.position = posData.worldPosition;
            Debug.Log(posData.worldRotation.y);
            Debug.Log(posData.worldRotation.eulerAngles.y);
            objprime.transform.rotation = Quaternion.Euler(objprime.transform.rotation.eulerAngles.x, posData.worldRotation.eulerAngles.y, objprime.transform.eulerAngles.z);
            
            objprime.transform.localScale = new Vector3(45, 45, 45);
            availablePositions.RemoveAt(index);

            if (FramesManager.Instance != null)
            {
                FramesManager.Instance.AddFrameProp(posData.roomName, objprime);
            }
        }

        while (availablePositions.Count > 0)
        {
            int index = Random.Range(0, availablePositions.Count);
            GameObject obj = objectTempoEnigme[Random.Range(0, objectTempoEnigme.Count)];
            PositionData posData = availablePositions[index];

            GameObject clone = Instantiate(obj, posData.worldPosition, Quaternion.Euler(obj.transform.rotation.eulerAngles.x, posData.worldRotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z));
            ApplyRandomColor(clone);

            //if (FramesManager.Instance != null)
            //{
            //    FramesManager.Instance.AddFrameProp(posData.roomName, clone);
            //}

            availablePositions.RemoveAt(index);

            instantiatedObjects.Add(clone); 
        }
    }
    private void ApplyRandomColor(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color randomColor = new Color(Random.value, Random.value, Random.value);
            renderer.material.color = randomColor;
        }
    }
    public void ClearClones()
    {
        foreach (GameObject clone in instantiatedObjects)
        {

            Destroy(clone); // Supprimer l'objet de la sc�ne
        }
        instantiatedObjects.Clear(); // Effacer la liste des clones
    }

    protected override void Fail()
    {
        start.SetActive(true);
        inGame.SetActive(false);
        ClearText();
        currentRound = 0;
        base.Fail();
    }
}
