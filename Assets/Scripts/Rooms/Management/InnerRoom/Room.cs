using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    [SerializeField] public RoomDataBase roomData;

    /// <summary>
    /// Called when scene is loaded. Will initialize the room.
    /// </summary>
    public virtual void Initialize()
    {

    }

    public void ReturnToHub()
    {
        TransitionManager.Instance.BackToExploration();
        //SceneManager.LoadScene("Pathfinding_1.0");
    }
}
