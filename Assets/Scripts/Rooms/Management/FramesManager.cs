using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FramesManager : MonoBehaviour
{
    [Header ("Unlocked")]
    [SerializeField] private Button upButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button downButton;
    [SerializeField] private Button leftButton;

    [Header("Locked")]
    [SerializeField] private Button upButtonLocked;
    [SerializeField] private Button rightButtonLocked;
    [SerializeField] private Button downButtonLocked;
    [SerializeField] private Button leftButtonLocked;

    [SerializeField] private TextMeshProUGUI ZoneText;


    [SerializeField] private Sprite lockedFrame;
    [SerializeField] private Sprite unlockedFrame;

    [Header("Bubble particles")]
    [SerializeField] private ParticleSystem randomParticleSystem;
    [SerializeField] private float maxParticleDepth = 25f;
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 5f;

    public event Action FrameSwitch;

    public static FramesManager Instance;

    [System.Serializable]
    public class Frame
    {
        public string id; // "Main_frame", "cave"...

        public GameObject PropsContainer;

        public Transform cameraPosition;
        public List <GameObject> ActiveProps; // Props being used in a frame

        public RoomStateEnum InitialFrameState;
        public RoomStateEnum FrameState;

        public List<FrameConnection> connections = new List<FrameConnection>();

    }
    [System.Serializable]
    public class FrameConnection
    {
        public DirectionsEnum direction;
 
        public string connectedFrameId;
    }

    public bool CameraSwap = false;

    public Frame currentFrame;
    public Frame[] frames;
    [SerializeField] private string initalFrame; //First frame always called main_frame

    private Coroutine currentCameraCoroutine;

    private Camera mainCamera;


    void Start()
    {
        foreach (var frame in frames)
        {
            frame.ActiveProps = new List<GameObject>();

            if (frame.PropsContainer != null) 
            {

                foreach (Transform child in frame.PropsContainer.transform)
                {
                    frame.ActiveProps.Add(child.gameObject);
                }
            }

           
        }

        mainCamera = Camera.main;
        SwitchFrame(initalFrame);
        StartCoroutine(SpawnParticlesRoutine());
        RandomParticleEffectLoop();

    }

    /// <summary>
    /// Plays bubble particle effect
    /// </summary>
    private void RandomParticleEffectLoop()
    {

            Vector3 randomPos = GetRandomPositionInView();
            randomParticleSystem.transform.position = randomPos;
            randomParticleSystem.gameObject.SetActive(true);
            randomParticleSystem.Play();
        
    }
    /// <summary>
    /// Coroutine waiting seconds before activating bubbles
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnParticlesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(8);
            RandomParticleEffectLoop();
        }
    }

    /// <summary>
    /// Get position within the screen view in world space to spawn bubble particles
    /// </summary>
    /// <returns></returns>
    Vector3 GetRandomPositionInView()
    {
        float depth = UnityEngine.Random.Range(15f, maxParticleDepth);
        float randomX = UnityEngine.Random.Range(0f, 1f);
        float randomY = UnityEngine.Random.Range(0f, 0.5f); 
        Vector3 viewportPos = new Vector3(randomX, randomY, depth);
        return Camera.main.ViewportToWorldPoint(viewportPos);
    }


    private void Awake()
    {
        if (Instance == null)
        {
            
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Add props to a frame.
    /// Parameters exepcting a frame id and a prop to add
    /// </summary>
    /// <param name="frameId"></param>
    /// <param name="prop"></param>
    public void AddFrameProp(string frameId, GameObject prop)
    {
        Frame targetFrame = System.Array.Find(frames, s => s.id == frameId);

        if (targetFrame  != null)
        {
            targetFrame.ActiveProps.Add(prop);
        }
        else
        {
            Debug.Log("frame not found");

        }
    }

    /// <summary>
    /// Clears prop list for a frame.
    /// Parameters exepcting a frame id.
    /// </summary>
    /// <param name="frameId"></param>
    /// <param name="prop"></param>
    public void ClearFramePropList(string frameId)
    {
        Frame targetFrame = System.Array.Find(frames, s => s.id == frameId);

        if (targetFrame != null)
        {
            targetFrame.ActiveProps.Clear();
        }
        else
        {
            Debug.Log("frame not found");

        }
    }
    /// <summary>
    /// Delete props to a frame.
    /// Parameters exepcting a frame id and a prop to delete
    /// </summary>
    /// <param name="frameId"></param>
    /// <param name="prop"></param>
    public void RemoveFrameProp(string frameId, GameObject prop)
    {
        Frame targetFrame = System.Array.Find(frames, s => s.id == frameId);

        if (targetFrame  != null)
        {
            targetFrame.ActiveProps.Remove(prop);
        }
        else
        {
            Debug.Log("frame not found");

        }
    }


    /// <summary>
    /// Switch frame. It deactivates non used props and active the used ones.
    /// Parameter expecting a string room ID.
    /// </summary>
    /// <param name="newRoomID"></param>
    public void SwitchFrame(string newRoomID)
    {
        if (CheckIfLocked(newRoomID))
        {
            //Target frame reference
            Frame targetFrame = System.Array.Find(frames, s => s.id == newRoomID);

            ZoneText.text = newRoomID;

            //Stop current camera movement
            if (currentCameraCoroutine != null)
            {
                StopCoroutine(currentCameraCoroutine);
                mainCamera.transform.position = currentFrame.cameraPosition.position;
            }


            //Manage props deactivation/activation
            ManageFrameProps(targetFrame);


            // Move camera
            currentCameraCoroutine= StartCoroutine(MoveCamera(targetFrame.cameraPosition));
            currentFrame = targetFrame;
            UpdateDirectionButtons();
            FrameSwitch?.Invoke();
        }
        else
        {
            Debug.Log("can't acces locked room");
        }

    }


    /// <summary>
    /// Switch camera position to the new target.
    /// Parameter expecting a vector 3 position.
    /// </summary>
    /// <param name="positionCible"></param>
    /// <returns></returns>
    IEnumerator MoveCamera(Transform targetTransform)
    {
        if (CameraSwap)
        {
            Camera.main.orthographic = !Camera.main.orthographic;
        }

        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        float elapsedTime = 0f;
        float duration = 0f;


        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(
                startPosition,
                targetTransform.position,
                elapsedTime / duration
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetTransform.position;
        mainCamera.transform.rotation = targetTransform.rotation;
    }


    /// <summary>
    /// Updates the switching frame buttons display depending on the current's frame connections
    /// </summary>
    void UpdateDirectionButtons()
    {
   
        // Reset all buttons
        upButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        downButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);

        upButtonLocked.gameObject.SetActive(false);
        downButtonLocked.gameObject.SetActive(false);
        leftButtonLocked.gameObject.SetActive(false);
        rightButtonLocked.gameObject.SetActive(false);

        // Set active buttons based on connections
        foreach (var connection in currentFrame.connections)
        {
            switch (connection.direction)
            {
                case DirectionsEnum.up:
                    upButton.gameObject.SetActive(true);
                    upButtonLocked.gameObject.SetActive(false);
                    Frame targetFrameUp = System.Array.Find(frames, s => s.id == connection.connectedFrameId);
                    if (targetFrameUp.FrameState == RoomStateEnum.Locked)
                    {
                        upButton.gameObject.SetActive(false);
                        upButtonLocked.gameObject.SetActive(true);

                    }
                    upButton.onClick.RemoveAllListeners();
                    upButton.onClick.AddListener(() => SwitchFrame(connection.connectedFrameId));
                    break;

                case DirectionsEnum.down:
                    downButton.gameObject.SetActive(true);
                    downButtonLocked.gameObject.SetActive(false);
                    Frame targetFrameDown = System.Array.Find(frames, s => s.id == connection.connectedFrameId);
                    if (targetFrameDown.FrameState == RoomStateEnum.Locked)
                    {
                        downButton.gameObject.SetActive(false);
                        downButtonLocked.gameObject.SetActive(true);
                    }
                    downButton.onClick.RemoveAllListeners();
                    downButton.onClick.AddListener(() => SwitchFrame(connection.connectedFrameId));
                    break;

                case DirectionsEnum.left:
                    leftButton.gameObject.SetActive(true);
                    leftButtonLocked.gameObject.SetActive(false);
                    Frame targetFrameLeft = System.Array.Find(frames, s => s.id == connection.connectedFrameId);
                    if (targetFrameLeft.FrameState == RoomStateEnum.Locked)
                    {
                        leftButton.gameObject.SetActive(false);
                        leftButtonLocked.gameObject.SetActive(true);
                    }
                    else
                    {
                        leftButton.image.sprite = unlockedFrame;
                    }
                    leftButton.onClick.RemoveAllListeners();
                    leftButton.onClick.AddListener(() => SwitchFrame(connection.connectedFrameId));

                    break;

                case DirectionsEnum.right:
                    rightButton.gameObject.SetActive(true);
                    rightButtonLocked.gameObject.SetActive(false);
                    Frame targetFrameRight = System.Array.Find(frames, s => s.id == connection.connectedFrameId);
                    if (targetFrameRight.FrameState == RoomStateEnum.Locked)
                    {
                        rightButton.gameObject.SetActive(false);
                        rightButtonLocked.gameObject.SetActive(true);
                    }
                    rightButton.onClick.RemoveAllListeners();
                    rightButton.onClick.AddListener(() => SwitchFrame(connection.connectedFrameId));
                    break;
                 
            }
        }

    }

    /// <summary>
    /// Checks if a frame is unlocked.
    /// </summary>
    /// <param name="frameId"></param>
    bool CheckIfLocked(string frameId)
    {
        Frame targetFrameUp = System.Array.Find(frames, s => s.id == frameId);

        if (targetFrameUp.FrameState == RoomStateEnum.Locked)
        {
            return false;
        }
        else
        {
            return true;
            
        }
    }

    void ManageFrameProps(Frame targetFrame)
    {
        
        if (targetFrame == null) return;

        // Deactivate all props
        foreach (var frame in frames)
        {
            foreach (var obj in frame.ActiveProps)
            {
                obj.SetActive(false);
            }
        }

        // Activate target frame and current frame props
        foreach (var obj in targetFrame.ActiveProps)
        {
            obj.SetActive(true);
        }
        foreach (var obj in currentFrame.ActiveProps)
        {
            obj.SetActive(true);
        }
    }

    public void UnlockFrame(string frame_id)
    {
        //Target frame reference
        Frame targetFrame = System.Array.Find(frames, s => s.id == frame_id);

        if (targetFrame != null)
        {
            targetFrame.FrameState = RoomStateEnum.Unlocked;
            UpdateDirectionButtons();
        }
        else
        {
            print("no frame");
        }
    }
    
    public void LockFrame(string frame_id)
    {
        //Target frame reference
        Frame targetFrame = System.Array.Find(frames, s => s.id == frame_id);

        if (targetFrame != null)
        {
            targetFrame.FrameState = RoomStateEnum.Locked;
            UpdateDirectionButtons() ;
        }
    }
}
