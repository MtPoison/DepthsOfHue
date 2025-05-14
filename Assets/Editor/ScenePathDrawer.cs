#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

[CustomPropertyDrawer(typeof(ScenePathAttribute))]
public class ScenePathDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        // Récupère toutes les scènes dans Assets/Scenes/Rooms
        string[] scenePaths = Directory.GetFiles("Assets/Scenes/Rooms", "*.unity", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray();

        if (scenePaths.Length == 0)
        {
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.HelpBox(position, "Aucune scène trouvée dans Assets/Scenes/Rooms", MessageType.Warning);
            return;
        }

        // Trouve l'index actuel
        int selectedIndex = Mathf.Max(0, System.Array.IndexOf(scenePaths, property.stringValue));

        // Affiche le menu déroulant
        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, scenePaths);
        property.stringValue = scenePaths[selectedIndex];
    }
}
#endif
