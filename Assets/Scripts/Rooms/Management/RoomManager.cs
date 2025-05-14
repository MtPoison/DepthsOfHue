using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Data")]
    private RoomDataBase currentRoom;
    [SerializeField] private RoomDataBase[] allRooms;
    [SerializeField] private Save sauvegarde;
    void Awake()
    {
        if (Instance == null) Instance = this; //Singleton instantiation
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (sauvegarde)
        {
            sauvegarde.LoadCategory("rooms");
        }
    }


    /// <summary>
    /// Main function to load a scene/room.
    /// Parameter is expecting a string room ID.
    /// </summary>
    ///<param name="roomId"></param>
    public void LoadRoom(string roomId)
    {
        RoomDataBase targetRoom = System.Array.Find(allRooms, roomData => roomData.roomId == roomId); //Searching room function

        if (targetRoom == null)
        {
            Debug.LogError($"Room {roomId} not found!");
            return;
        }

        currentRoom = targetRoom;
        
        SceneManager.sceneLoaded += OnSceneLoaded; // On scene loaded event subscription
        TransitionManager.Instance.StartEnigme(targetRoom.sceneName);
        //SceneManager.LoadScene(targetRoom.sceneName); //Load the scene
    }


    /// <summary>
    /// Called when the event scene loaded is done
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    /// 
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentRoom != null)
        {
            if (scene.name != currentRoom.sceneName) return;

            // Finds the room controller script in the scene
            Room roomController = FindObjectOfType<Room>();

            if (roomController != null)
            {
                if (roomController.roomData == null) 
                {
                    roomController.roomData = currentRoom;
                }
               
                roomController.Initialize();
            
            }
            else
            {
                Debug.LogWarning("No RoomController found in scene!");
            }
            DialogueManager.Instance.StopCurrentDialogue();
            if (!currentRoom.GetIsVisited())
            {
                currentRoom.VisitRoom();
                DialogueManager.Instance.StartNewRoomDialogue();
                sauvegarde.SaveCategory("rooms");
            }
            SceneManager.sceneLoaded -= OnSceneLoaded; // On scene loaded event unsubscription
        }
        
    }

    /// <summary>
    /// Getter for room data.
    /// Paremeter is expecting a string room ID.
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    public RoomDataBase GetRoomData(string roomId)
    {
        RoomDataBase targetRoom = System.Array.Find(allRooms, roomData => roomData.roomId == roomId); //Searching room function
        if (targetRoom == null)
        {
            Debug.LogError($"Room {roomId} not found!");
            return null;
        }

        return targetRoom;
    }


    /// <summary>
    /// Unlocks a room.
    /// Parameter is expecting a string room ID
    /// </summary>
    /// <param name="roomId"></param>
    public void UnlockRoom(string roomId)
    {

        RoomDataBase room = GetRoomData(roomId);
        if (room != null)
        {
            room.CurrentState = RoomStateEnum.Unlocked;
            Debug.Log($"Room {roomId} unlocked !");

            room.roomState = room.CurrentState;
            Debug.Log(room.CurrentState.ToString());
        }
    }
    
    private void OnApplicationQuit()
    {
        foreach (var so in allRooms)
        {
            if (so != null)
            {
                so.isVisited = false; 
            }
        }
    }

    public List<string> GetIdAllRooms()
    {
        List<string> roomIds = new List<string>();
        foreach (var so in allRooms)
        {
            roomIds.Add(so.roomId);
        }
        return roomIds;
    }

    public List<bool> GetAlreadyVisitedBoolList()
    {
        List<bool> visited = new List<bool>();
        foreach (var so in allRooms)
        {
            visited.Add(so.isVisited);
        }

        return visited;
    }

    public RoomDataBase[] GetAllRooms()
    {
        return allRooms;
    }
}
