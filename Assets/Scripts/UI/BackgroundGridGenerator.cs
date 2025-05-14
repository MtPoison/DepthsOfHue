using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGridGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] imagePrefab;
    [SerializeField] private Sprite[] backgroundSprites;
    [SerializeField] private int columns = 3;
    [SerializeField] private int rows = 3;
    private Camera mainCamera;
    private GameObject[,] gridCadres;
    [SerializeField] private bool isForBackgroundLayer;
    [SerializeField] private ShowMap showMap;
    [SerializeField] private Save save;
    [SerializeField] private GameObject navMeshCenter;
    
    public List<GameObject> backgrounds = new List<GameObject>();
    public List<GestionCadre> cadres = new List<GestionCadre>();

    #region Event

    public delegate void OnFinishSpawnCadres();
    public static event OnFinishSpawnCadres OnSpawnCadre;

    public delegate void SendCadre(GestionCadre cadre);
    public static event SendCadre OnSendCadre;
    
    public delegate void SendSetupDoors();
    public static event SendSetupDoors OnSendSetupDoors;

    #endregion

    private void OnEnable()
    {
        if (backgrounds.Count > 0) return;
        
        mainCamera = Camera.main;
        Vector2 screenSize = GetScreenSizeInUnits();
        
        if (navMeshCenter)
        {
            Vector3 posNavmesh = navMeshCenter.transform.position;
            posNavmesh.x = screenSize.x * 2;
            posNavmesh.x += 1f;
            navMeshCenter.transform.position = posNavmesh;
        }
        
        gridCadres = new GameObject[columns, rows];

        for (int i = 0; i < backgroundSprites.Length; i++)
        {
            int x = i % columns;
            int y = i / columns;

            Vector3 position = new Vector3(x * screenSize.x, -y * screenSize.y, 0);
            GameObject go = Instantiate(imagePrefab[i], position, Quaternion.identity, transform);
            backgrounds.Add(go);

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = backgroundSprites[i];

            Vector3 scale = go.transform.localScale;
            scale.x = screenSize.x / sr.sprite.bounds.size.x;
            scale.y = screenSize.y / sr.sprite.bounds.size.y;
            go.transform.localScale = scale;

            gridCadres[x, y] = go;
        }

        if (isForBackgroundLayer) return;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject cadre = gridCadres[x, y];
                if (!cadre) continue;
                
                GestionCadre gestionCadre = cadre.GetComponent<GestionCadre>();
                if (!gestionCadre) continue;
                OnSendCadre?.Invoke(gestionCadre);
                
                cadres.Add(gestionCadre);

                #region Attribution des cadre target à tous les cadres selon les directions coché

                bool isArrowLeft = gestionCadre.ArrowLeftBool;
                bool isArrowRight = gestionCadre.ArrowRightBool;
                bool isArrowDown = gestionCadre.ArrowDownBool;
                bool isArrowUp = gestionCadre.ArrowUpBool;

                // Exceptions
                bool isExceptionCaseLeft = (x == columns - 1 && y == rows - 1) || (x == 2 && y == 0);
                bool isExceptionCaseRight = (x == columns - 1 && y == rows - 1) || (x == 2 && y == 0);
                bool isExceptionCaseDown = (x == columns - 1 && y == rows - 1) || (x == 2 && y == 0);
                bool isExceptionCaseUp = (x == columns - 1 && y == rows - 1) || (x == rows - 3 && y == columns - 1);

                // permet de set les target cadre selon leur position dans la grid
                // Variables pour vérifier les directions
                if (isArrowLeft && (x > 0 || isExceptionCaseLeft)) gestionCadre.TargetCadreLeftGO = gridCadres[x - 1, y];
                if (isArrowRight && (x < columns - 1 || isExceptionCaseRight)) gestionCadre.TargetCadreRightGO = gridCadres[x + 1, y];
                if (isArrowDown && (y < rows - 1 || isExceptionCaseDown)) gestionCadre.TargetCadreDownGO = gridCadres[x, y + 1];
                if (isArrowUp && (y > 0 || isExceptionCaseUp)) gestionCadre.TargetCadreUpGO = gridCadres[x, y - 1];

                #endregion
                
                OnSpawnCadre?.Invoke();
            }
        }
        if (showMap) showMap.SetReceiveFromBGGridGenerator(cadres);
    }

    private void Start()
    {
        OnSendSetupDoors?.Invoke();
    }

    private Vector2 GetScreenSizeInUnits()
    {
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        return new Vector2(width, height);
    }
}