using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[System.Serializable]
public class PuzzleData
{
    public ItemData itemData;
    public string prefabPath;  // Chemin du prefab dans Resources
    public int row;
    public int col;
    public string id;
}
public class Inventaire : MonoBehaviour
{
    [SerializeField] private List<PuzzleData> inventaireFragment;
    [SerializeField] private List<ItemData> inventaire;
    [SerializeField] private UI_Inventaire inv;
    [SerializeField] private List<ItemData> itemDatas;
    [SerializeField] private ParticleSystem part;
    [SerializeField] private Save save;
    private List<string> ids = new List<string>();


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }

    
    void Start()
    {
        InputItems.Instance.OnClickOnGameObject += HandleObjectClick;
        save.LoadCategory("inventory");
        // string searchTerm = "perle";
        // List<ItemData> matchingItems = FindItemsByPartialName( searchTerm);

        // foreach (ItemData item in matchingItems)
        // {
        //     Debug.Log("Objet trouv� : " + item.itemName);
        // }

    }
    private void HandleObjectClick(GameObject go)
    {

        AnItem anItem = go.GetComponent<AnItem>();

        if (anItem != null && anItem.itemData != null)
        {

            Add(anItem.itemData, go);
        }
    }
    public void Add(ItemData item, GameObject obj)
    {
        if (/*inv.GetUnmodifiedSprite() > 0 || */item.type.ToString() == "Stack" || (!inventaire.Contains(item) && item.type.ToString() != "Stack"))
        {


            
                inventaire.Add(item);

                Debug.Log(item.prefab.ToString());
            
            //GameObject obje =Instantiate(item.prefab);

            //MonoBehaviour d = obje.AddComponent<PuzzleFragment>();

            
            //d = item.behaviours[0];
                     
            //inv.UpdateUI();
            Destroy(obj);
   
            //part.transform.position = obj.transform.position;
            //part.Play();
            save.SaveCategory("inventory");
            Debug.Log(item.itemName + " ajouté à l'inventaire.");
        }
        else
        {
            Debug.Log(item.itemName + " est d�j� dans l'inventaire.");
        }
    }

    public void Remove( ItemData item)
    {
        if (inventaire.Contains(item))
        {
            inventaire.Remove(item);
            inv.UpdateUI();
            Debug.Log(item.itemName + " retir� de l'inventaire.");
        }
        else
        {
            Debug.Log(item.itemName + " n'est pas dans l'inventaire.");
        }
    }

    public List<ItemData> FindItemsByPartialName( string partialName)
    {
        List<ItemData> foundItems = inventaire.Where(item => item.itemName.IndexOf(partialName, System.StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        if (foundItems.Count == 0)
        {
            throw new System.Exception("Aucun objet trouv� contenant : " + partialName);
        }
        return foundItems;
    }

    public ItemData GetItemByName(string itemName)
    {
        ItemData item = inventaire.FirstOrDefault(item => item.itemName.Equals(itemName, System.StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            throw new System.Exception("Aucun objet trouv� avec le nom : " + itemName);
        }
        return item;
    }

    public List<Sprite> GetAllSprites()
    {
        // if (inventaire.Count == 0)
        // {
        //     throw new System.Exception("L'inventaire est vide, aucun sprite disponible.");
        // }
        List<Sprite> sprites = inventaire.Select(item => item.itemSprite).Where(sprite => sprite != null).ToList();
        if (sprites.Count == 0)
        {
            throw new System.Exception("Aucun sprite valide trouv� dans l'inventaire.");
        }
        return sprites;
    }

    public List<ItemData> GetInventaire() {  return inventaire; }
    public List<string> GetId()
    {
        ids.Clear();
        if(inventaire.Count == 0) 
        { 
            
            return null; 
        }
        foreach(var item in inventaire) 
        { 
            ids.Add(item.GetId());
        }
        return ids;
    }

    public void SetId(List<string> _ids) { 
        print(_ids);
        ids = _ids; 
    }
    public void SetInventaire(List<ItemData> _inventaire) { inventaire = _inventaire; }
    public void AddItemSave()
    {
        for (int i = 0; i < ids.Count; i++)
        {
            for(int j = 0; j < itemDatas.Count; j++)
            {
                if (ids[i] == itemDatas[j].GetId())
                {
                    inventaire.Add(itemDatas[j]);
                }
            }
        }
    }

   
    public bool HasFragments()
    {
        return inventaire.Count > 0;
    }

    public bool HasAllFragment()
    {
        return inventaire.Count >= 6;
    }

    public GameObject InstantiateNextFragment()
    {
        if (inventaire.Count == 0)
            return null;

        GameObject nextFragment = inventaire[0].prefab;

        

        GameObject instance = Instantiate(nextFragment, Vector3.zero, Quaternion.identity);

      
        return instance;
    }

    public void RemoveFirstFragment()
    {
        if (inventaire.Count > 0)
        {
            // Supprime le premier élément de la liste
            inventaire.RemoveAt(0);
            save.SaveCategory("inventory"); // Sauvegarde la modification
            Debug.Log("Premier fragment retiré de l'inventaire");
        }
        else
        {
            Debug.LogWarning("Aucun fragment à supprimer - inventaire vide");
        }
    }
    public List<ItemData> GetItems() { return itemDatas; }
}
