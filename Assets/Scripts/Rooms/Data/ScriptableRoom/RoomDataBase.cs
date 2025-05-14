using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Game/Rooms/Data", order = 1)]
[System.Serializable]
public class RoomDataBase : ScriptableObject
{
    [Header("Identification")]
    public string roomId;

    [ScenePath] 
    public string sceneName;

    public bool isVisited = false;
    
    [Header("Default State")]
    public RoomStateEnum initialState;

    public RoomStateEnum roomState;

    // Propriété avec sauvegarde automatique
    public RoomStateEnum CurrentState
    {
        get => (RoomStateEnum)PlayerPrefs.GetInt(roomId + "_state", (int)initialState);
        set => PlayerPrefs.SetInt(roomId + "_state", (int)value);

    }

    public void VisitRoom()
    {
        isVisited = true;
    }

    public bool GetIsVisited()
    {
        return isVisited;
    }
}
