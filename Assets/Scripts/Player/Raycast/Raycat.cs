using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Raycat : MonoBehaviour
{
    public static event Action OnClickOnNothing;
    public static event Action<GameObject> OnClickOnGameObject;

    private Vector3 positionObj;
    private GameObject Obj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) // Clic souris (PC)
        //{
        //    DetectAndExecute(Input.mousePosition);
        //}

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        {
        
            DetectAndExecute(Input.GetTouch(0).position);
        }
    }

    void DetectAndExecute(Vector2 screenPosition)
    {
        
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
   
        if (Physics.Raycast(ray, out hit))
        {
            GameObject go = hit.transform.gameObject;

            if (go != null)
            {
               
                OnClickOnGameObject?.Invoke(go);
                
            }
        }
        else
        {
            OnClickOnNothing?.Invoke();
        }
    }

    public Vector3 GetPosition() { return  positionObj; }
    public GameObject GetObj() { return Obj; }
}
