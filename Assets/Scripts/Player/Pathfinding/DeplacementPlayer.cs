using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class DeplacementPlayer : MonoBehaviour
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");

    [Header("Property")]
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator compagnon;
    [SerializeField] private Transform targetCompagnon;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    [Header("Animations List")]
    [SerializeField] private List<string> listAnimations;
    public List<string> ListAnimations { get => listAnimations; set => listAnimations = value; }
    
    [SerializeField] private Vector3 playerDestination;
    private GestionCadre actualCadre;

    private Camera _camera;
    private bool uniqueSendEvent;
    private bool isForDeplacementEnigme;
    private DoorController actualDoor;

    #region Gestion Bool Gestion Cadre For Animation

    private bool playerPressRightArrow = false;
    private bool playerPressLeftArrow = false;
    private bool playerPressUpArrow = false;
    private bool playerPressDownArrow = false;
    public bool PlayerPressLeftArrow { get => playerPressLeftArrow; set => playerPressLeftArrow = value; }
    public bool PlayerPressRightArrow { get => playerPressRightArrow; set => playerPressRightArrow = value; }
    public bool PlayerPressUpArrow { get => playerPressUpArrow; set => playerPressUpArrow = value; }
    public bool PlayerPressDownArrow { get => playerPressDownArrow; set => playerPressDownArrow = value; }
    
    public Animator Compagnon { get => compagnon; set => compagnon = value; }
    public Transform TargetCompagnon { get => targetCompagnon; set => targetCompagnon = value; }

    #endregion

    #region Event

    public delegate void ShowUIGame(bool _isShow);
    public static event ShowUIGame OnShowUI;

    #endregion
    
    private void Start()
    {
        _camera = Camera.main;
        player.freezeRotation = true;
        uniqueSendEvent = false;
        isForDeplacementEnigme = false;
        
        Animator animator = GetComponent<Animator>();
        animator.SetBool(IsWalk, true);
        compagnon.SetBool(IsWalk, true);
    }

    private void OnEnable()
    {
        GestionInputs.OnPlayerGoFront += SetPlayerDestination;
    }
    
    private void OnDisable()
    {
        GestionInputs.OnPlayerGoFront -= SetPlayerDestination;
    }

    public void MovePlayer()
    {
        if (!navMeshAgent.enabled) return;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.SetDestination(playerDestination);
        isForDeplacementEnigme = false;
        CheckTargetCinemachine();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(playerDestination, Vector3.one * 1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(playerDestination, 0.3f);
    }

    private void Update()
    {
        // pas de rotation chelou sur ces axes
        Vector3 rotation = transform.eulerAngles;
        rotation.x = 0;
        rotation.y = 0;
        rotation.z = 0;
        transform.eulerAngles = rotation;

        if (!navMeshAgent.isOnNavMesh) return;

        if (!isForDeplacementEnigme)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (actualDoor)
                {
                    isForDeplacementEnigme = true;
                    actualDoor.OnClicked();
                }
            }
        }
        
        if (isForDeplacementEnigme) return;
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!actualCadre) return;
            if (!uniqueSendEvent)
            {
                actualCadre.SetArrowsVisibilities();
                OnShowUI?.Invoke(true);
                uniqueSendEvent = true;
            }
        }
    }

    private void CheckTargetCinemachine()
    {
        if (cinemachineVirtualCamera && cinemachineVirtualCamera.Follow) return;
        cinemachineVirtualCamera.Follow = transform;
    }
    
    public void SetPlayerDestination(Vector3 _playerDestination, GestionCadre _cadre)
    {
        playerDestination = _playerDestination;
        actualCadre = _cadre;
        uniqueSendEvent = false;
    }
    
    private void SetPlayerDestination(Vector3 _playerDestination, DoorController _doorController, int _direction)
    {
        playerDestination = _playerDestination;
        uniqueSendEvent = false;
        actualDoor = _doorController;
        MovePlayer();
        SetAnimationBasedOnDirection(_direction);
    }

    private void SetAnimationBasedOnDirection(int _direction)
    {
        Animator animator = GetComponent<Animator>();
        if (!animator) return;
        
        switch (_direction)
        {
            case 0:
                ResetBoolAnimation("IsWalk");
                break;
            case 1:
                ResetBoolAnimation("IsRight");
                break;
            case -1:
                ResetBoolAnimation("IsLeft");
                break;
            case -2:
                ResetBoolAnimation("IsDown");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
    
    private void ResetBoolAnimation(string _animToSkip)
    {
        if (!player) return;
        Animator animator = GetComponent<Animator>();
        if (!animator) return;
        
        foreach (var t in listAnimations)
        {
            if (t == _animToSkip)
            {
                animator.SetBool(Animator.StringToHash(t), true);
                continue;
            }
            animator.SetBool(Animator.StringToHash(t), false);
        }
    }
}
