using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class Pillar : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private Enigme_Pillar spawner;
    [SerializeField] private GestionInputs ray;


    private GameObject Obj;
    private bool isObj;
    private string Id;
    private static string previousClickedObj;

    public GameObject Popup
    {
        get => popup;
        set => popup = value;
    }

    public Enigme_Pillar Spawner
    {
        get => spawner;
        set => spawner = value;
    }

    public GestionInputs Ray
    {
        get => ray;
        set => ray = value;
    }

    public GameObject Objet
    {
        get => Obj;
        set => Obj = value;
    }

    public bool IsObj
    {
        get => isObj;
        set => isObj = !IsObj;
    }

    public string ID
    {
        get => Id;
        set => Id = value;
    }



    void OnEnable()
    {
        GestionInputs.OnClickOnNothing += HandleClickOnNothing;
     
    }

    public void OnObjectClicked(GameObject pillar)
    {

        string currentClickedObj = pillar.GetComponent<Pillar>().ID;

        if (currentClickedObj != null)
        {
            Debug.Log("y'a un componentn");
        }

        if (Input.touchCount > 0 &&
            EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) &&
            popup.activeSelf &&
            previousClickedObj == currentClickedObj)
        {
            
            return;
        }
        previousClickedObj = currentClickedObj;

        /*spawner.UpdatePopup();*/

        if (popup == null || gameObject == null)
        {
            Debug.LogWarning("Popup ou targetObject est null !");
            return;
        }

        popup.SetActive(true);
        Vector3 worldPosition = gameObject.transform.position + Vector3.up * 2f;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.position = new Vector2(screenPosition.x, screenPosition.y+30);

        if (!spawner.firstPillarClicked)
        {
            spawner.firstPillarClicked = true;
            DialogueManager.Instance.StartEnterPuzzleRoom(spawner.enigmeDialogKey);
            spawner.ShowIndiceButton();
        }
    }

    void HandleClickOnNothing()
    {
        print("return false");
            
        popup.SetActive(false);
    }



    public void SetTake()
    {
        isObj = !isObj;
    }

}