using System;
using UnityEngine;

public class GetMiddleCadre : MonoBehaviour
{
    [SerializeField] private Transform center;
    private string cadre;
    
    public delegate void GetMiddleCadreFunc(Transform middle);
    public static event GetMiddleCadreFunc OnGetMiddleCadre;
    
    public delegate void CalculNewRotation();
    public static event CalculNewRotation OnCalculNewRotation;

    private void OnEnable()
    {
        if (center) OnGetMiddleCadre?.Invoke(center);

        ManagerNavigateCadre.OnSendTargetStringCadre += SetStringCadreTarget;
        GestionCadre.OnSendTargetStringCadre += SetStringCadreTarget;
    }

    private void OnDisable()
    {
        ManagerNavigateCadre.OnSendTargetStringCadre -= SetStringCadreTarget;
        GestionCadre.OnSendTargetStringCadre -= SetStringCadreTarget;
    }

    private void SetStringCadreTarget(string _cadre)
    {
        cadre = _cadre;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(cadre) && cadre.Length <= 0) return;
        if (!other.CompareTag("Player")) return;
        OnCalculNewRotation?.Invoke();
        cadre = "";
    }
}
