using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public static HubManager Instance;

    [Header("References")]
    [SerializeField] private DoorController[] doors;
    [SerializeField] private GameObject playerSpawnPoint;
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

    private void OnEnable()
    {
        BackgroundGridGenerator.OnSendSetupDoors += FoundDoors;
    }

    private void OnDisable()
    {
        BackgroundGridGenerator.OnSendSetupDoors -= FoundDoors;
    }

    private void Start()
    {
        InitializeDoors();
        CheckUnlockFinalRoom();
    }

    /// <summary>
    /// Initialize each door in the room, resulting in a state update + visual update
    /// </summary>
    private void InitializeDoors()
    {
        if(doors != null)
        {        
            foreach (DoorController door in doors)
            {
                door.Initialize(); // Initialize each door
            }
        }
    }

    private void FoundDoors()
    {
        doors = GameObject.FindObjectsOfType<DoorController>(true)
            .Where(d => d.gameObject.CompareTag("Doors"))
            .ToArray();
    }

   private void CheckUnlockFinalRoom()
    {
        Inventaire inventory = FindObjectOfType<Inventaire>();

        if (inventory == null)
        {
            Debug.LogWarning("InventoryManager not found in the scene!");
            return;
        }

        if (inventory.HasAllFragment())
        {
            foreach (DoorController door in doors)
            {
                if (door.lastRoom)
                {
                    door.gameObject.SetActive(true);
                    break;
                }
            }
        }

        
    }
}
