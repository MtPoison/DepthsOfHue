using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

[Serializable]
public class DialogueData
{
    public string npc;
    public Sprite image;
    public string dialogueKey;
}

public enum DialogueGroupKey
{
    startGame,
    chantsSirene,
    carteDestin,
    introspection,
    newRoom,
    sudokuLike,
    start,
    chantsSireneHint,
    carteDestinHint,
    findObject,
    findObjectHint,
    memory,
    pillar,
    pillarHint
}

[System.Serializable]
public class DialogueKeyPair
{
    public DialogueGroupKey key;
    public List<DialogueGroup> value;
}

public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private TextMeshProUGUI nameComponent;
    [SerializeField] private Image imageComponent;

    [SerializeField] private GameObject dialogFrame;
    [SerializeField] private AudioClip dialogSound;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    [ReorderableList] public List<DialogueKeyPair> listDialogues;
    private DialogueGroup currentDialogue;
    
    private StringTable dialogueTable;
    private int index = 0;
    private bool isBusy = false;
    
    public float timeBeforeInactivityText = 15f;
    private float currentTimer;
    private bool isCountingDown = true;

    public bool enteredBaseMap = false;
    [SerializeField] private Save sauvegarde;

    public static DialogueManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        if (sauvegarde)
        {
            sauvegarde.LoadCategory("dialogbasemap");
        }
        if (dialogFrame != null)
        {
            dialogFrame.SetActive(false);
        }

        if (textComponent != null)
        {
            textComponent.text = string.Empty;
        }

        if (listDialogues != null)
        {
            foreach (var dialogueGroupList in listDialogues)
            {
                foreach (var dialogue in dialogueGroupList.value)
                {
                    LoadLocalizationFromTable(dialogue.tableKey);
                }
            }

            if (!enteredBaseMap)
            {
                enteredBaseMap = true;
                StartDialogue(0,DialogueGroupKey.start);
                sauvegarde.SaveCategory("dialogbasemap");
            }
           
        }
    }

    private void Update()
    {
        InactivityTimer();
    }

    public void LoadLocalizationFromTable(string tableKey)
    {
        dialogueTable = LocalizationSettings.StringDatabase.GetTable(tableKey);
    }

    public string GetDialogue(DialogueData dialogueData)
    {
        if (dialogueTable != null)
        {
            var localizedString = dialogueTable.GetEntry(dialogueData.dialogueKey);
            return localizedString != null ? localizedString.LocalizedValue : "Dialogue not found.";
        }
        else
        {
            Debug.LogWarning("Dialogue is not loaded.");
            return "Dialogue unavailable";
        }
    }

    IEnumerator TypeLine(DialogueData dialogueData)
    {
        nameComponent.text = dialogueData.npc;
        imageComponent.color = new Color(1, 1, 1, dialogueData.image == null ? 0 : 1);
        imageComponent.sprite = dialogueData.image;
        foreach (char c in GetDialogue(dialogueData))
        {
            textComponent.text += c;
            audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(dialogSound);

            if (c == '.') {
                yield return new WaitForSeconds(0.5f);
            } else if (c == ',') {
                yield return new WaitForSeconds(0.25f);
            }

            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(1);
    }

    public IEnumerator WriteDialogue(DialogueGroup dialogueGroup)
    {
        if (isBusy)
            yield break;

        isBusy = true;
        index = 0;
        textComponent.text = string.Empty;
        dialogFrame.SetActive(true);

        foreach (DialogueData dialogue in dialogueGroup.dialogues)
        {
            yield return StartCoroutine(TypeLine(dialogue));

            index++;
            textComponent.text = string.Empty;
        }

        dialogFrame.SetActive(false);
        isBusy = false;
    }
    
    public bool IsBusy()
    {
        return isBusy;
    }
    
    public void StartDialogue(int idDialogue,DialogueGroupKey keyGroup)
    {
        if (listDialogues == null) return;
        foreach (var dialoguePair in listDialogues)
        {
            if (dialoguePair.key == keyGroup)
            {
                currentDialogue = dialoguePair.value[idDialogue];
            }
        }
        StartCoroutine(WriteDialogue(currentDialogue));
    }

    private void InactivityTimer()
    {
        if (!isCountingDown) return;
        currentTimer += Time.deltaTime;
        if (!(currentTimer >= timeBeforeInactivityText)) return;
        System.Random rand = new System.Random();
        int countMax = 0;
        foreach (var dialoguePair in listDialogues)
        {
            if (dialoguePair.key == DialogueGroupKey.introspection)
            {
                countMax = dialoguePair.value.Count;
                break;
            }
        }
        int randId = rand.Next(0, countMax);
        StartDialogue(randId,DialogueGroupKey.introspection);
        currentTimer = 0f;
    }
    
    public void StopCurrentDialogue()
    {
        if (!isBusy) return;
        
        StopAllCoroutines();
        
        textComponent.text = string.Empty;
        dialogFrame.SetActive(false);
        isBusy = false;
        index = 0;
    }
    
    public void StartNewDialogue(int idDialogue, DialogueGroupKey keyGroup)
    {
        StopCurrentDialogue();
        foreach (var dialoguePair in listDialogues)
        {
            if (dialoguePair.key == keyGroup)
            {
                currentDialogue = dialoguePair.value[idDialogue];
                break;
            }
        }
    
        StartCoroutine(WriteDialogue(currentDialogue));
    }

    public void StartNewRoomDialogue()
    {
        StartNewDialogue(0,DialogueGroupKey.newRoom);
    }

    public void StartEnterPuzzleRoom(DialogueGroupKey puzzleKey)
    {
        StartNewDialogue(0,puzzleKey);
    }
}
