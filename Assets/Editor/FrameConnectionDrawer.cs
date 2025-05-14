using UnityEditor;
using UnityEngine;
using static FramesManager;

[CustomPropertyDrawer(typeof(FrameConnection))]
public class FrameConnectionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Récupère les références
        var directionProp = property.FindPropertyRelative("direction");
        var frameIdProp = property.FindPropertyRelative("connectedFrameId");
        var frames = GetFramesList(property);

        // Calcule les rectangles
        Rect directionRect = new Rect(position.x, position.y, position.width * 0.4f, position.height);
        Rect frameRect = new Rect(position.x + position.width * 0.45f, position.y, position.width * 0.55f, position.height);

        // Affiche les champs
        EditorGUI.PropertyField(directionRect, directionProp, GUIContent.none);

        if (frames.Length > 0)
        {
            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(frames, frameIdProp.stringValue));
            selectedIndex = EditorGUI.Popup(frameRect, selectedIndex, frames);
            frameIdProp.stringValue = frames[selectedIndex];
        }
        else
        {
            EditorGUI.PropertyField(frameRect, frameIdProp, GUIContent.none);
        }

        EditorGUI.EndProperty();
    }

    private string[] GetFramesList(SerializedProperty property)
    {
        // Remonte jusqu'au FramesManager parent
        var managerProp = property.serializedObject.FindProperty("frames");
        if (managerProp == null) return new string[0];

        string[] frames = new string[managerProp.arraySize];
        for (int i = 0; i < managerProp.arraySize; i++)
        {
            var element = managerProp.GetArrayElementAtIndex(i);
            frames[i] = element.FindPropertyRelative("id").stringValue;
        }

        return frames;
    }
}