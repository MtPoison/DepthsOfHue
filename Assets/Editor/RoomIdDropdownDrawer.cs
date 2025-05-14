// RoomIdDropdownDrawer.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic; // Ajoutez cette ligne
using System.Linq; // Et cette ligne si vous utilisez LINQ

[CustomPropertyDrawer(typeof(RoomIdAttribute))]
public class RoomIdDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Trouve tous les RoomDataBase disponibles
        string[] roomIds = GetAllRoomIds();

        if (roomIds.Length == 0)
        {
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.HelpBox(position, "Aucune room cr��e !", MessageType.Warning);
            return;
        }

        // Trouve l'index actuel
        int selectedIndex = Mathf.Max(0, System.Array.IndexOf(roomIds, property.stringValue));

        // Affiche le menu d�roulant
        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, roomIds);
        property.stringValue = roomIds[selectedIndex];
    }

    private string[] GetAllRoomIds()
    {
        // R�cup�re tous les assets de type RoomDataBase
        string[] guids = AssetDatabase.FindAssets("t:RoomDataBase");
        List<string> ids = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            RoomDataBase room = AssetDatabase.LoadAssetAtPath<RoomDataBase>(path);
            if (room != null && !string.IsNullOrEmpty(room.roomId))
            {
                ids.Add(room.roomId);
            }
        }

        return ids.ToArray();
    }
}
#endif
