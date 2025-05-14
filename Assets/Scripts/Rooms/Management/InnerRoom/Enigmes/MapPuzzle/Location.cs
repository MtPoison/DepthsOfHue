using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MapLocations
{
    megalodon,
    epave,
    petrol,
    kraken,
    atlantis,
    island
}

public class Location : MonoBehaviour
{
    public MapLocations locationName;
    public bool isVisited;

    private GameObject bubbleSelected;

    private void Start()
    {
        bubbleSelected = transform.GetChild(1).gameObject;
        bubbleSelected.SetActive(false);
    }

    public void VisitLoc()
    {
        isVisited = true;
        bubbleSelected.SetActive(true);
    }

    public void UnvisitLoc()
    {
        isVisited = false;
        bubbleSelected.SetActive(false);
    }
}
