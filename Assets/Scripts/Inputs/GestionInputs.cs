using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;


public class GestionInputs : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private Controls controls;
    private Camera _camera;

    public static event Action OnClickOnNothing;
    public static event Action<GameObject> OnClickOnGameObject;

    private Vector3 positionObj;
    private GameObject Obj;

    private bool canUpdateGestionInputs;

    #region Event

    public delegate void PlayerGoFrontEnigme(Vector3 _position, DoorController _doorController, int _direction);
    public static event PlayerGoFrontEnigme OnPlayerGoFront;

    #endregion

    private void Awake()
    {      

        EnhancedTouchSupport.Enable();
      
    }

    private void OnEnable()
    {
        TouchSimulation.Enable();
        AnimScale.OnCanUpdateGestionInputs += SetCanUpdateGestionInputs;
    }

    private void OnDisable()
    {
        AnimScale.OnCanUpdateGestionInputs -= SetCanUpdateGestionInputs;
    }

    private void Start()
    {
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        _camera = Camera.main;
        canUpdateGestionInputs = false;
    }

    private void SetCanUpdateGestionInputs(bool _canUpdate)
    {
        canUpdateGestionInputs = _canUpdate;
    }

    private void Update()
    {
        if (canUpdateGestionInputs) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition = Input.mousePosition;
            MapNavigateCadre();
            StartEnigme(touchPosition);
        }
    }

    public Vector3 GetPosition() { return positionObj; }
    public GameObject GetObj() { return Obj; }

    private void MapNavigateCadre()
    {
        Vector3 touchPosition = Input.mousePosition;
        Ray ray = _camera.ScreenPointToRay(touchPosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (!hit.collider) return;
        if (hit.collider.CompareTag("Ancre"))
        {
            MapNavigateCadre hitMapNavigate = hit.collider.GetComponent<MapNavigateCadre>();
            if (hitMapNavigate) hitMapNavigate.ClickMapNavigate();
        }
    }
    
    private void StartEnigme(Vector3 _touchPosition)
    {
        Ray ray = _camera.ScreenPointToRay(_touchPosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (!hit.collider) return;
        if (hit.collider.CompareTag("Doors"))
        {
            DoorController hitDoorController = hit.collider.GetComponent<DoorController>();
            OnPlayerGoFront?.Invoke(hitDoorController.gameObject.transform.position, hitDoorController, hitDoorController.Direction);
            if (cinemachineVirtualCamera) cinemachineVirtualCamera.Follow = null;
        }
    }

    /*public Vector3 GetPosition() { return positionObj; }
    public GameObject GetObj() { return Obj; }*/
}
