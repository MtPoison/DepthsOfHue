using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.SceneManagement;

public class InputItems : MonoBehaviour
{
    private Camera _camera;

    public  event Action OnClickOnNothing;
    public  event Action<GameObject> OnClickOnGameObject;

    public static InputItems Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        EnhancedTouchSupport.Enable();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        _camera = Camera.main;
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        TouchSimulation.Enable();
    }

    private void Start()
    {
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var touch in Touch.activeTouches)
        {

            if (touch.isTap)
            {

                Vector3 touchPosition = touch.screenPosition;
                Ray ray = _camera.ScreenPointToRay(touchPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

                    GameObject go = hit.transform.gameObject;

                    if (go != null)
                    {
                        OnClickOnGameObject?.Invoke(go);


                        return;
                    }
                    //MonoBehaviour script = hit.collider.GetComponent<MonoBehaviour>();

                    //Collider collider = hit.collider;

                    //Obj = collider.gameObject;

                    //positionObj = collider.bounds.center + new Vector3(0, collider.bounds.extents.y, 0);

                    //if (script != null)
                    //{
                    //    script.Invoke("OnObjectClicked", 0f);

                    //}

                    // if (hit.collider.CompareTag("Ancre"))
                    // {
                    //     MapNavigateCadre hitMapNavigate = hit.collider.GetComponent<MapNavigateCadre>();
                    //     if (hitMapNavigate) hitMapNavigate.ClickMapNavigate();
                    // }
                }
                else
                {
                    OnClickOnNothing?.Invoke();
                    return;
                }
            }
        }
    }
}
