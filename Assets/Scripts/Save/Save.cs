using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Save : MonoBehaviour
{
    private string savePath;
    [SerializeField] Inventaire inventaire;
    [SerializeField] ShowMap showMap;
    [SerializeField] AudioOptionManager audio;
    [SerializeField] RoomManager rooms;
    [SerializeField] DialogueManager dialogueManager;
    #region Event

    public delegate void SaveStartGamePlayer();
    public static event SaveStartGamePlayer OnSaveStartPlayer;
    
    public delegate void SaveStartGameActualCadre();
    public static event SaveStartGameActualCadre OnSaveStartActualCadre;

    #endregion
    private void Awake()
    {
        savePath = Application.persistentDataPath + "/gameSave.json";
        // if (inventaire == null)
        // {
        //     Debug.LogError("Inventaire not found in the scene!");
        // }
        EnsureSaveFileExists();
    }

    private void OnEnable()
    {
        AudioOptionManager.OnSendStartLoadAudioToSave += LoadAudio;
    }
    
    private void OnDisable()
    {
        AudioOptionManager.OnSendStartLoadAudioToSave -= LoadAudio;
    }

    private void LoadAudio()
    {
        if (audio) LoadCategory("audio");
    }

    private void EnsureSaveFileExists()
    {
        if (!File.Exists(savePath))
        {
            SaveAllCategories();
            Debug.Log("New save file created at: " + savePath);
        }
    }

    public void SaveCategory(string category)
    {
        SaveData saveData = LoadExistingData();

        switch (category.ToLower())
        {
            case "inventory":
                saveData.inventoryData = new InventoryData
                {
                    scriptableObjectIDs = inventaire.GetId()
                };
                break;
            // Add more categories as needed
            case "mapcadre":
                saveData.mapData = new MapData
                {
                    mapInfo = ConvertDictToList(showMap.GetMapStatus())
                };
                break;
            case "explorationcadre":
                saveData.cadreData = new CadreData
                {
                    actualCadre = showMap.ActualCadre
                };
                break;
            case "audio":
                saveData.audiomanager = new Audio
                {
                    music = audio.MusicSlider.value,
                    soundEffect = audio.SoundEffectsSlider.value,
                    isSave = true
                };
                break;
            case "rooms":
                saveData.roomData = new RoomData
                {
                    roomDataScriptableIDs = rooms.GetIdAllRooms(),
                    roomDataAlreadyEntered = rooms.GetAlreadyVisitedBoolList()
                };
                break;
            case "dialogbasemap":
                saveData.dialBaseMapData = new DialBaseMapData
                {
                    alreadyDialogueBaseMap = dialogueManager.enteredBaseMap
                };
                break;
            default:
                Debug.LogWarning($"Category {category} not recognized!");
                return;
        }

        SaveToFile(saveData);
        Debug.Log($"Category {category} saved to: {savePath}");
    }

    public void SaveAllCategories()
    {
        SaveData saveData = new SaveData
        {
            inventoryData = new InventoryData
            {
                scriptableObjectIDs = inventaire != null ? inventaire.GetId() : new List<string>()
            },
            mapData = new MapData
            {
                mapInfo =  showMap != null ? ConvertDictToList(showMap.GetMapStatus()) : new List<SerializableKeyValuePair>()
            },
            cadreData = new CadreData
            {
                actualCadre = showMap != null ? showMap.ActualCadre : ""
            },
            audiomanager = new Audio
            {
                music = audio != null ? audio.MusicSlider.value : 1f,
                soundEffect = audio != null ? audio.SoundEffectsSlider.value : 1f,
            },
            roomData = new RoomData
            {
                roomDataScriptableIDs = rooms != null ? rooms.GetIdAllRooms() : new List<string>(),
                roomDataAlreadyEntered = rooms != null ? rooms.GetAlreadyVisitedBoolList() : new List<bool>()
            },
            dialBaseMapData = new DialBaseMapData
            {
                alreadyDialogueBaseMap = dialogueManager != null && dialogueManager.enteredBaseMap
            }
            
        };

        SaveToFile(saveData);
        Debug.Log($"All categories saved to: {savePath}");
    }

    public void LoadCategory(string category)
    {
        SaveData saveData = LoadExistingData();
        switch (category.Trim().ToLower())
        {
            case "inventory":
                if (saveData.inventoryData != null && inventaire)
                {
                    inventaire.SetId(saveData.inventoryData.scriptableObjectIDs);
                    inventaire.AddItemSave();
                    
                }
                break;
            case "mapcadre":
                if (saveData.mapData != null && showMap)
                {
                    if (saveData.mapData.mapInfo == null || saveData.mapData.mapInfo.Count == 0)
                    {
                        Debug.Log("New Save Cadre");
                        OnSaveStartPlayer?.Invoke();
                    }
                    else
                    {
                        showMap.SetMapStatus(ConvertListToDict(saveData.mapData.mapInfo));
                    }
                }
                break;
            case "explorationcadre":
                if (saveData.cadreData != null && showMap)
                {
                    if (string.IsNullOrEmpty(saveData.cadreData.actualCadre) || saveData.cadreData == null)
                    {
                        Debug.Log("New Save Actual Cadre");
                        OnSaveStartActualCadre?.Invoke();
                    }
                    else
                    {
                        print(saveData.cadreData.actualCadre);
                        showMap.SetActualCadre(saveData.cadreData.actualCadre);
                    }
                }
                break;
            case "audio":
                if(saveData.audiomanager != null && audio)
                {
                    audio.MusicSlider.value = saveData.audiomanager.music;
                    audio.SoundEffectsSlider.value = saveData.audiomanager.soundEffect;
                    audio.IsLoad = saveData.audiomanager.isSave;

                }
                break;
            case "rooms":
                LoadRooms();
                break;
            case "dialogbasemap":
                if (saveData.dialBaseMapData != null && dialogueManager)
                {
                    dialogueManager.enteredBaseMap = saveData.dialBaseMapData.alreadyDialogueBaseMap;
                    Debug.Log("load " + dialogueManager.enteredBaseMap);
                }
                break;
            default:
                Debug.LogWarning($"Category {category} not recognized!");
                return;
        }

        Debug.Log($"Category {category} loaded from: {savePath}");
    }

    public void LoadAllCategories()
    {
        SaveData saveData = LoadExistingData();

        if (saveData.inventoryData != null)
        {
            inventaire.SetId(saveData.inventoryData.scriptableObjectIDs);
            inventaire.AddItemSave();
        }

        /*if (saveData.playerData != null)
        {
            // Apply player data here
            // Example: dataManager.SetPlayerPosition(saveData.playerData.position);
        }*/
        if (saveData.audiomanager != null)
        {
            audio.MusicSlider.value = saveData.audiomanager.music;
            audio.SoundEffectsSlider.value = saveData.audiomanager.soundEffect;
            audio.IsLoad = saveData.audiomanager.isSave;

        }

        if (saveData.roomData != null)
        {
            LoadRooms();
        }

        Debug.Log($"All categories loaded from: {savePath}");
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            SaveAllCategories();
            Debug.Log("Save file deleted and recreated at: " + savePath);
        }
    }

    private SaveData LoadExistingData()
    {

        if (!File.Exists(savePath))
        {
            return new SaveData(); // <-- vide !
        }
        
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    private void SaveToFile(SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
    }
    
    private List<SerializableKeyValuePair> ConvertDictToList(Dictionary<string, bool> dict)
    {
        var list = new List<SerializableKeyValuePair>();
        foreach (var kvp in dict)
        {
            list.Add(new SerializableKeyValuePair { key = kvp.Key, value = kvp.Value });
        }
        return list;
    }

    private Dictionary<string, bool> ConvertListToDict(List<SerializableKeyValuePair> list)
    {
        var dict = new Dictionary<string, bool>();
        foreach (var kvp in list)
        {
            dict[kvp.key] = kvp.value;
        }
        return dict;
    }

    private void LoadRooms()
    {
        SaveData saveData = LoadExistingData();
        
        for (int i = 0; i < saveData.roomData.roomDataScriptableIDs.Count; i++)
        {
            string roomid = saveData.roomData.roomDataScriptableIDs[i];
            foreach (var room in rooms.GetAllRooms())
            {
                if (room.roomId == roomid)
                {
                    room.isVisited = saveData.roomData.roomDataAlreadyEntered[i];
                }
            }
        }
    }

    [System.Serializable]
    private class SaveData
    {
        public InventoryData inventoryData;
        public MapData mapData;
        public CadreData cadreData;
        public Audio audiomanager;
        public RoomData roomData;
        public DialBaseMapData dialBaseMapData;
    }

    [System.Serializable]
    private class InventoryData
    {
        public List<string> scriptableObjectIDs;
    }
    
    [System.Serializable]
    private class RoomData
    {
        public List<string> roomDataScriptableIDs;
        public List<bool> roomDataAlreadyEntered;
    }
    
    [System.Serializable]
    private class DialBaseMapData
    {
        public bool alreadyDialogueBaseMap;
    }

    [System.Serializable]
    private class Audio
    {
        public float music;
        public float soundEffect;
        public bool isSave;
    }

    #region Serialize Map Cadre Data

    [System.Serializable]
    public class SerializableKeyValuePair
    {
        public string key;
        public bool value;
    }

    [System.Serializable]
    private class MapData
    {
        public List<SerializableKeyValuePair> mapInfo;
    }

    #endregion

    #region Serialize Cadre Spawn Player

    [System.Serializable]
    private class CadreData
    {
        public string actualCadre;
    }

    #endregion
}