using System.Collections.Generic;
using UnityEngine;

public class MapBackgroundUI : MonoBehaviour
{
    [SerializeField] private GameObject[] imagePrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private Sprite[] backgroundSprites;
    [SerializeField] private int columns = 3;
    [SerializeField] private int rows = 3;
    private Camera cam;
    private GameObject[,] gridCadres;
    
    public List<GameObject> backgrounds = new List<GameObject>();
    public List<GameObject> cadresMap = new List<GameObject>();
    [SerializeField] private ShowMap showMap;

    private void OnEnable()
    {
        cam = Camera.main;
        if (backgrounds.Count > 0)
        {
            UpdateBackgroundPositions();
            ReAdujstPosition();
            return;
        }

        Vector2 screenSize = GetScreenSizeInUnits(); // taille de l’écran en unités monde
        Vector2 cellSize = new Vector2(screenSize.x / columns, screenSize.y / rows);
        Vector2 origin = new Vector2(-screenSize.x / 2f, -screenSize.y / 2f); // coin bas gauche **relatif au centre de caméra**

        Vector3 cameraCenter = cam.transform.position;
        cameraCenter.z = 0f; // on reste en 2D

        gridCadres = new GameObject[columns, rows];

        for (int i = 0; i < backgroundSprites.Length; i++)
        {
            int x = i % columns;
            int y = i / columns;
            if (y >= rows) break;

            // position dans la cellule, en partant du coin inférieur gauche du champ de vision caméra
            Vector3 localPosition = new Vector3(
                origin.x + cellSize.x * (x + 0.5f),
                origin.y + cellSize.y * (y + 0.5f),
                0f);

            Vector3 worldPosition = cameraCenter + localPosition;

            GameObject go = Instantiate(imagePrefab[i], worldPosition, Quaternion.identity, transform);
            backgrounds.Add(go);
            if (go.CompareTag("MapCadre")) cadresMap.Add(go);

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = backgroundSprites[i];

            float scaleX = cellSize.x / sr.sprite.bounds.size.x;
            float scaleY = cellSize.y / sr.sprite.bounds.size.y;
            go.transform.localScale = new Vector3(scaleX, scaleY, 1f);

            gridCadres[x, y] = go;
        }
        
        showMap.SetMapCadre(cadresMap);
        ReAdujstPosition();
    }

    private Vector2 GetScreenSizeInUnits()
    {
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        return new Vector2(width, height);
    }

    private void UpdateBackgroundPositions()
    {
        if (backgrounds.Count != columns * rows) return;

        Vector2 screenSize = GetScreenSizeInUnits();
        Vector2 cellSize = new Vector2(screenSize.x / columns, screenSize.y / rows);
        Vector2 origin = new Vector2(-screenSize.x / 2f, -screenSize.y / 2f);

        Vector3 cameraCenter = cam.transform.position;
        cameraCenter.z = 0f;

        for (int i = 0; i < backgrounds.Count; i++)
        {
            int x = i % columns;
            int y = i / columns;
            if (y >= rows) break;

            Vector3 localPosition = new Vector3(
                origin.x + cellSize.x * (x + 0.5f),
                origin.y + cellSize.y * (y + 0.5f),
                0f);

            Vector3 worldPosition = cameraCenter + localPosition;

            GameObject bg = backgrounds[i];
            bg.transform.position = worldPosition;

            SpriteRenderer sr = bg.GetComponent<SpriteRenderer>();
            float scaleX = cellSize.x / sr.sprite.bounds.size.x;
            float scaleY = cellSize.y / sr.sprite.bounds.size.y;
            bg.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
    }

    private void ReAdujstPosition()
    {
        Vector3 pos = gameObject.transform.position;
        pos.y += 0.4f;
        gameObject.transform.position = pos;
    }
}