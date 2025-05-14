using System.Collections.Generic;
using UnityEngine;

public class StockArrowVisible : MonoBehaviour
{
    private List<GameObject> arrows = new List<GameObject>();
    
    private void OnEnable()
    {
        ArrowsTrigger.OnSendArrow += AddArrows;
        AnimScale.OnShowArrowsHidden += ShowArrows;
    }
    
    private void OnDisable()
    {
        ArrowsTrigger.OnSendArrow -= AddArrows;
        AnimScale.OnShowArrowsHidden -= ShowArrows;
    }

    private void AddArrows(GameObject _arrow)
    {
        arrows.Add(_arrow);
        _arrow.SetActive(false);
    }
    
    private void ShowArrows()
    {
        foreach (var arrow in arrows)
        {
            ArrowsTrigger arrowsTrigger = arrow.GetComponent<ArrowsTrigger>();
            arrowsTrigger.CanDoAction = true;
            arrow.SetActive(true);
        }
        arrows.Clear();
    }
}
