using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1 : EnigmeRoom
{
    
    [ContextMenu("Initialize room1")]
    
    public override void Initialize()
    {
        foreach (var enigme in enigmes)// subscribe to each enigme OnSucces event.
        {
            enigme.OnSuccess -= OnEnigmeResolved; // if already subscribed
            enigme.OnSuccess += OnEnigmeResolved;
        }
    }

}

