using System.Collections.Generic;
using UnityEngine;

public class ShowCanvasExplorationScene : MonoBehaviour
{
    [SerializeField] private List<GameObject> canvas;

    private void OnEnable()
    {
        DeplacementPlayer.OnShowUI += SetShowCanvas;
        MapNavigateCadre.OnShowUI += SetShowCanvas;
        GestionCadre.OnShowUI += SetShowCanvas;
    }
    
    private void OnDisable()
    {
        DeplacementPlayer.OnShowUI -= SetShowCanvas;
        MapNavigateCadre.OnShowUI -= SetShowCanvas;
        GestionCadre.OnShowUI -= SetShowCanvas;
    }

    private void SetShowCanvas(bool _show)
    {
        if (canvas.Count <= 0) return;
        
        foreach (var t in canvas)
        {
            t.SetActive(_show);
        }
    }
}
