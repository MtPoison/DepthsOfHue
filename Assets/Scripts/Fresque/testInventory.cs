using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testInventory : MonoBehaviour
{
    public static testInventory Instance;
    public HashSet<string> collectedFragments = new HashSet<string>();
    public List<PuzzleFragment> collectedFragment;
    public int indexer = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddFragment(PuzzleFragment fragment)
    {
        fragment.transform.SetParent(transform);
        collectedFragment.Add(fragment);
        indexer++;

        PuzzleManager.Instance.AddFragmentToFresque(fragment);
          
    }

    public bool HasFragment(string id)
    {
        return collectedFragments.Contains(id);
    }

    public IEnumerable<string> GetAllFragments() => collectedFragments;
}
