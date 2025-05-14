using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [RoomId] 
    [SerializeField] private string targetRoomId;
    [SerializeField] private bool isLocked;
    [SerializeField] private GameObject lockedVisual;
    [SerializeField] private GameObject unlockedVisual;
    [SerializeField] private int direction;

    public bool lastRoom = false;

    #region Getter
    public int Direction => direction;

    #endregion
    
    /// <summary>
    /// Called by HUB manager, initialize door depending on its state
    /// </summary>
    public void Initialize()
    {
        //Check room's state
        RoomDataBase targetRoom = RoomManager.Instance.GetRoomData(targetRoomId);
        isLocked = (targetRoom.CurrentState == RoomStateEnum.Locked);

        // Update visuals

        if (lockedVisual != null && unlockedVisual != null)
        {
            UpdateDoorVisual();

        }
        
        Debug.Log($"Porte {targetRoomId} initialis�e : Verrouill�e = {isLocked}");
    }


    /*private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    OnClicked();
                }
            }
        }
    }*/

    /// <summary>
    /// Update the door visuals.
    /// </summary>
    private void UpdateDoorVisual()
    {
        if (lockedVisual != null) lockedVisual.SetActive(isLocked);
        if (unlockedVisual != null) unlockedVisual.SetActive(!isLocked);
    }

    /// <summary>
    /// Called when door is clicked. If not locked, will load the attached room.
    /// </summary>
    public void OnClicked()
    {
        if (!isLocked)
        {
            RoomManager.Instance.LoadRoom(targetRoomId);
        }
        else
        {
            RoomManager.Instance.UnlockRoom(targetRoomId);
            Debug.Log("Door Locked !");
        }
    }
   

}
