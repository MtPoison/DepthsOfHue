using System;
using UnityEngine;

public class MapNavigateCadre : MonoBehaviour
{
    [SerializeField] private string referenceCadre;
    private ShowMap showMap;

    #region Event

    public delegate void OnHideMap();
    public static event OnHideMap OnHide;
    
    public delegate void OnMovePlayer(string _referenceCadre);
    public static event OnMovePlayer OnMove;
    
    public delegate void ShowUIGame(bool _isShow);
    public static event ShowUIGame OnShowUI;

    #endregion

    private void OnEnable()
    {
        ShowMap.OnSendShowMapEvent += SetShowMap;
    }

    private void OnDisable()
    {
        ShowMap.OnSendShowMapEvent -= SetShowMap;
    }

    private void SetShowMap(ShowMap _showMap)
    {
        showMap = _showMap;
    }

    public void ClickMapNavigate()
    {
        if (!CheckCanNavigate(referenceCadre)) return;
        OnHide?.Invoke();
        OnShowUI?.Invoke(false);
        if (referenceCadre.Length > 0) OnMove?.Invoke(referenceCadre);
    }

    private bool CheckCanNavigate(string _referenceCadre)
    {
        if (!showMap) return false;

        foreach (var destination in showMap.GetMapStatus())
        {
            string key = destination.Key;
            bool value = destination.Value;
            
            if (!key.Equals(_referenceCadre)) continue;
            if (key.Equals(referenceCadre) && value)
            {
                return true;
            }
        }

        return false;
    }
}
