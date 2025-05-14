using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ShowMap : MonoBehaviour
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");

    [Header("Show Map")]
    [SerializeField] List<GameObject> objToHide = new List<GameObject>();
    [SerializeField] List<GameObject> objToShow = new List<GameObject>();
    
    [Header("Generator Grids Gestion")]
    [SerializeField] private BackgroundGridGenerator gridGenerator;
    [SerializeField] private MapBackgroundUI mapBackgroundUI;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator compagnonAnimator;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject compagnon;
    
    private Dictionary<string, bool> statusMap = new Dictionary<string, bool>();
    [Header("Sauvegarde")]
    [SerializeField] private Save sauvegarde;
    private List<GestionCadre> cadres = new List<GestionCadre>();
    private List<GameObject> cadresMap = new List<GameObject>();
    private bool receiveFromBGGridGenerator = false;
    private bool receiveFromSauvegarde = false;
    private string actualCadre;
    
    public string ActualCadre { get => actualCadre;
        private set => actualCadre = value; }
    
    [Header("Materials")]
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material blurMaterial;
    
    [Header("Sprites")]
    [SerializeField] private Sprite ancre;
    [SerializeField] private Sprite lockedBubble;

    private bool isOpen;

    #region Event

    public delegate void SendShowMapEvent(ShowMap _showMap);
    public static event SendShowMapEvent OnSendShowMapEvent;

    #endregion

    private void Start()
    {
        isOpen = false;
        if (sauvegarde)
        {
            sauvegarde.LoadCategory("explorationcadre");
            sauvegarde.LoadCategory("mapcadre");
        }
    }

    private void OnEnable()
    {
        MapNavigateCadre.OnHide += ClickMapIcon;
        Save.OnSaveStartPlayer += SetReceiveFromSauvegarde;
        Save.OnSaveStartActualCadre += SetActualCadreFirstSave;
        GestionCadre.OnSendNewStatus += ModifyStatusCadre;
        ManagerNavigateCadre.OnSendNewStatus += ModifyStatusCadre;
    }

    private void OnDisable()
    {
        MapNavigateCadre.OnHide -= ClickMapIcon;
        Save.OnSaveStartPlayer -= SetReceiveFromSauvegarde;
        Save.OnSaveStartActualCadre -= SetActualCadreFirstSave;
        GestionCadre.OnSendNewStatus -= ModifyStatusCadre;
        ManagerNavigateCadre.OnSendNewStatus -= ModifyStatusCadre;
    }

    public void ClickMapIcon()
    {
        if (objToHide.Count <= 0 || objToShow.Count <= 0) return;
        foreach (var hide in objToHide) hide.SetActive(isOpen);
        foreach (var show in objToShow) show.SetActive(!isOpen);

        if (gridGenerator.backgrounds.Count > 0)
        {
            foreach (var mainGrid in gridGenerator.backgrounds) mainGrid.SetActive(isOpen);
        }
        
        if (mapBackgroundUI.backgrounds.Count > 0)
        {
            foreach (var mainGrid in mapBackgroundUI.backgrounds) mainGrid.SetActive(!isOpen);
        }

        if (isOpen && playerAnimator && compagnonAnimator)
        {
            playerAnimator.SetBool(IsWalk, true);
            compagnonAnimator.SetBool(IsWalk, true);
        }

        UpdateStatusCadre();
        OnSendShowMapEvent?.Invoke(this);
        
        isOpen = !isOpen;
    }

    private void SetReceiveFromSauvegarde()
    {
        receiveFromSauvegarde = true;
        SaveStartingPlayer();
    }

    public void SetReceiveFromBGGridGenerator(List<GestionCadre> _cadres)
    {
        receiveFromBGGridGenerator = true;
        cadres = _cadres;
        SaveStartingPlayer();
    }

    private void SaveStartingPlayer()
    {
        if (!sauvegarde) return;
        if (!receiveFromSauvegarde || !receiveFromBGGridGenerator) return;
        bool foundActualCadre = false;

        foreach (var cadre in cadres)
        {
            if (cadre.gameObject.CompareTag("ActualCadre"))
            {
                statusMap.Add(cadre.gameObject.name, true);
                foundActualCadre = true;
            }
            else
            {
                statusMap.Add(cadre.gameObject.name, false);
            }
        }

        if (!foundActualCadre)
        {
            var defaultCadre = cadres.FirstOrDefault(t => t.name == "CadreHautTemple(Clone)");
            if (defaultCadre)
            {
                statusMap.Add(defaultCadre.gameObject.name, true);
            }
        }

        sauvegarde.SaveCategory("mapcadre");
    }
    
    public void SetMapStatus(Dictionary<string, bool> _mapInfo)
    {
        statusMap = _mapInfo;
    }
    
    public Dictionary<string, bool> GetMapStatus()
    {
        return statusMap;
    }

    private void ModifyStatusCadre(GestionCadre _cadre, GameObject _go = null)
    {
        if (_go)
        {
            ActualCadre = _go.name;
            sauvegarde.SaveCategory("explorationcadre");
        }
        if (!statusMap.ContainsKey(_cadre.gameObject.name)) return;
        statusMap[_cadre.gameObject.name] = true;
        sauvegarde.SaveCategory("mapcadre");
        UpdateStatusCadre();
    }
    
    public void SetMapCadre(List<GameObject> _cadres)
    {
        cadresMap.AddRange(_cadres);
    }

    private void UpdateStatusCadre()
    {
        sauvegarde.LoadCategory("mapcadre");
        
        foreach (var t in cadresMap)
        {
            if (!t) return;
            foreach (var cadre in statusMap)
            {
                string key = cadre.Key;
                key = "Map" + key;
                
                bool value = cadre.Value;

                if (t.name == key)
                {
                    t.GetComponent<InformationCadreMap>().Material.material = value ? originalMaterial : blurMaterial;
                    t.GetComponent<InformationCadreMap>().Locked.sprite = value ? ancre : lockedBubble;
                }
            }
        }
    }

    private void SetActualCadreFirstSave()
    {
        var defaultCadre = cadres.FirstOrDefault(t => t.name == "CadreHautTemple(Clone)");
        if (!defaultCadre) return;
        defaultCadre.tag = "ActualCadre";
        TeleportPlayer(defaultCadre);
        ActualCadre = "CadreHautTemple(Clone)";
        sauvegarde.SaveCategory("explorationcadre");
    }

    public void SetActualCadre(string _actualCadre)
    {
        bool found = false;
        
        foreach (var t in cadres.Where(t => t.name == _actualCadre))
        {
            t.tag = "ActualCadre";
            TeleportPlayer(t);
            found = true;
            break;
        }

        if (found) return;
        {
            var defaultCadre = cadres.FirstOrDefault(t => t.name == "CadreHautTemple(Clone)");
            if (!defaultCadre) return;
            defaultCadre.tag = "ActualCadre";
            TeleportPlayer(defaultCadre);
        }
    }

    private void TeleportPlayer(GestionCadre _cadre)
    {
        player.transform.position = _cadre.center.position;
        compagnon.GetComponent<NavMeshAgent>().updatePosition = false;
        compagnon.transform.position = player.GetComponent<DeplacementPlayer>().TargetCompagnon.position;
        compagnon.GetComponent<NavMeshAgent>().updatePosition = true;
        _cadre.SetArrowsVisibilities();
        _cadre.StockVisiblities();
    }
}
