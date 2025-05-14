using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class DialogueGroup : ScriptableObject
{
    public List<DialogueData> dialogues;
    public string tableKey;
}