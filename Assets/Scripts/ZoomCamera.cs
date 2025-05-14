using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public float minZoomSize = 3f;
    public float defaultSize = 5f;
    public float zoomStep = 0.5f;
    public float zoomSpeed = 5f;
    public float doubleClickTime = 0.3f;
    public float dragSpeed = 0.1f;

    private Camera cam;
    private float targetSize;
    private float lastClickTime;
    private bool isZoomed = false;
    private Vector2 dragOrigin;
    private bool isDragging = false;
    private Vector3 initialPosition;
    private Vector2 defaultMoveLimits;

    void Start()
    {
        cam = Camera.main;
        defaultSize = cam.orthographicSize;
        targetSize = defaultSize;
        initialPosition = transform.position;

        defaultMoveLimits = new Vector2(defaultSize * cam.aspect, defaultSize);
    }

    void Update()
    {
        initialPosition = transform.position;
        if (cam.orthographicSize >= 4.99f)
        {
            transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * 5f);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                targetSize = Mathf.Max(minZoomSize, targetSize - zoomStep);
                isZoomed = (targetSize < defaultSize);
            }
            lastClickTime = Time.time;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                targetSize = Mathf.Min(defaultSize, targetSize + zoomStep);
                isZoomed = (targetSize < defaultSize);
            }
            lastClickTime = Time.time;
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

        Vector2 currentMoveLimits = new Vector2(targetSize * cam.aspect, targetSize);
        Vector2 adjustedLimits = defaultMoveLimits - currentMoveLimits;

        if (isZoomed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
                isDragging = true;
            }
            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector2 difference = dragOrigin - (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
                Vector3 newPosition = transform.position + (Vector3)difference * dragSpeed;

                newPosition.x = Mathf.Clamp(newPosition.x, initialPosition.x - adjustedLimits.x, initialPosition.x + adjustedLimits.x);
                newPosition.y = Mathf.Clamp(newPosition.y, initialPosition.y - adjustedLimits.y, initialPosition.y + adjustedLimits.y);
                transform.position = newPosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
        
    }
}