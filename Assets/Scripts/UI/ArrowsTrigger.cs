using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArrowsTrigger : MonoBehaviour
{
    [SerializeField] private GestionCadre cadre;
    [SerializeField] private GameObject targetArrow;
    
    private bool canDoAction;
    public bool CanDoAction { get => canDoAction; set => canDoAction = value; }

    #region Event

    public delegate void SendArrow(GameObject _arrow);
    public static event SendArrow OnSendArrow;

    #endregion
    private void Start()
    {
        canDoAction = true;
        EventTrigger trigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    private void OnEnable()
    {
        AnimScale.OnSendArrowsToArrowsTrigger += SetCanDoAction;
    }

    private void OnDisable()
    {
        AnimScale.OnSendArrowsToArrowsTrigger -= SetCanDoAction;
    }
    
    private void SetCanDoAction(bool _canDoAction)
    {
        canDoAction = _canDoAction;
        if (gameObject.activeSelf) OnSendArrow?.Invoke(gameObject);
    }

    public void OnPointerDownDelegate(PointerEventData data)
    {
        if (!canDoAction) return;
        cadre.NavigateCadre(targetArrow);
    }
}
