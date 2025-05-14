using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiddleRoom : EnigmeRoom
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Initialize();
        DialogueManager.Instance.StartEnterPuzzleRoom(enigmes[0].enigmeDialogKey);
    }

    
    public override void EndRoomSequence()
    {
        base.EndRoomSequence();

        ReturnToHub();
    }
    
    protected override void OnPostEnigme()
    {
        if (IsRoomComplete())
        {
            EndRoomSequence();
        }
        else
        {
            
            InitilizeCurrentEnigma();
        }
    }

}
