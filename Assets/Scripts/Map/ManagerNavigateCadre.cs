using System;
using System.Collections.Generic;
using UnityEngine;

public class ManagerNavigateCadre : MonoBehaviour
{
    [SerializeField] private DeplacementPlayer player;
    [SerializeField] private BackgroundGridGenerator backgroundGridGenerator;
    private List<GestionCadre> allCadres = new List<GestionCadre>();
    private GestionCadre actualCadre;
    private GestionCadre targetCadre;
    
    public delegate void SendNewStatus(GestionCadre _cadre, GameObject _go);
    public static event SendNewStatus OnSendNewStatus;
    
    public delegate void SendTargetStringCadre(string _cadre);
    public static event SendTargetStringCadre OnSendTargetStringCadre;

    private void Start()
    {
        AddCadre();
    }

    private void OnEnable()
    {
        MapNavigateCadre.OnMove += MapNavigateCadreFunc;
        GetMiddleCadre.OnCalculNewRotation += CalculNewRotation;
    }

    private void OnDisable()
    {
        MapNavigateCadre.OnMove -= MapNavigateCadreFunc;
        GetMiddleCadre.OnCalculNewRotation -= CalculNewRotation;
    }

    private void AddCadre()
    {
        allCadres = backgroundGridGenerator.cadres;
        FoundActualCadre();
    }

    private void FoundActualCadre()
    {
        foreach (var cadre in allCadres)
        {
            if (cadre.CompareTag("ActualCadre")) actualCadre = cadre;
        }
    }
    
    private void MapNavigateCadreFunc(string _targetCadre)
    {
        GameObject cadre = CompareName(_targetCadre);
        FoundActualCadre();
        OnSendTargetStringCadre?.Invoke(cadre.name);
        if (!cadre) Debug.LogError("cadre null");
        actualCadre.gameObject.tag = "Untagged";
        
        GestionCadre gestionCadre = cadre.GetComponent<GestionCadre>();
        if (!gestionCadre) Debug.LogError("gestionCadre null");

        targetCadre = gestionCadre;
        gestionCadre.ResetArrows();
        gestionCadre.ResetAllArrows();

        // le middle cadre - l'actual cadre (universel pour toute direction)
        ManageRotation(cadre);
        
        player.SetPlayerDestination(cadre.transform.TransformPoint(gestionCadre.center.localPosition), gestionCadre);
        player.MovePlayer();
        cadre.gameObject.tag = "ActualCadre";
        OnSendNewStatus?.Invoke(gestionCadre, cadre);

        gestionCadre.ManageRotationMovement(cadre, true);
    }
    
    private void ManageRotation(GameObject _cadre)
    {
        GestionCadre gestionCadre = _cadre.GetComponent<GestionCadre>();
        if (!gestionCadre.centerMiddle) FindCenterMiddle(gestionCadre);

        var heading = (GestionException(_cadre) ? gestionCadre.center.position : gestionCadre.centerMiddle.position) - actualCadre.transform.position;
        var distance = heading.magnitude;
        Vector3 directionTemp = heading / distance;
        gestionCadre.SetDirection = new Vector3(
            Mathf.RoundToInt(directionTemp.x),
            Mathf.RoundToInt(directionTemp.y),
            Mathf.RoundToInt(directionTemp.z)
        );
        
        gestionCadre.SetTargetCadre = _cadre;
    }
    
    private void FindCenterMiddle(GestionCadre _cadre)
    {
        GameObject centerMiddleFound = GameObject.FindGameObjectWithTag("centerMidCadre");
        _cadre.centerMiddle = centerMiddleFound.transform;
    }
    
    private GameObject CompareName(string _target)
    {
        if (allCadres.Count == 0) AddCadre();
        foreach (var kvp in allCadres)
        {
            if (kvp.name != _target) continue;
            return kvp.gameObject;
        }
        return null;
    }
    
    private void CalculNewRotation()
    {
        // le target cadre - l'actual cadre
        if (!targetCadre) return;
        if (targetCadre.name == "CadreMidTemple(Clone)") return;

        var heading = targetCadre.center.position - targetCadre.centerMiddle.position;
        var distance = heading.magnitude;
        Vector3 directionTemp = heading / distance;
        targetCadre.SetDirection = new Vector3(
            Mathf.RoundToInt(directionTemp.x),
            Mathf.RoundToInt(directionTemp.y),
            Mathf.RoundToInt(directionTemp.z)
        );

        targetCadre.ManageRotationMovement(targetCadre.gameObject, true);
    }

    #region Exception

    private bool GestionException(GameObject _cadre)
    {
        if (_cadre.name == "CadreRightTemple(Clone)" && actualCadre.gameObject.name == "CadreRightTemple2(Clone)") return true;
        if (actualCadre.gameObject.name == "CadreRightTemple(Clone)" && _cadre.name == "CadreRightTemple2(Clone)") return true;
        
        if (_cadre.name == "CadreHautTemple(Clone)" && actualCadre.gameObject.name == "CadreHautTemple2(Clone)") return true;
        if (actualCadre.gameObject.name == "CadreHautTemple2(Clone)" && _cadre.name == "CadreHautTemple(Clone)") return true;
        
        if (_cadre.name == "CadreBotTemple(Clone)" && actualCadre.gameObject.name == "CadreBotTemple2(Clone)") return true;
        if (actualCadre.gameObject.name == "CadreBotTemple2(Clone)" && _cadre.name == "CadreBotTemple(Clone)") return true;

        return false;
    }

    #endregion
}
