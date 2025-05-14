using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(GestionCadre))]
public class OptionsManagerEditor : Editor
{
    private Dictionary<string, (SerializedProperty Arrow, SerializedProperty TagTarget, SerializedProperty Target, SerializedProperty ArrowObj, SerializedProperty Angle)> arrowMappings;

    private void OnEnable()
    {
        arrowMappings = new Dictionary<string, (SerializedProperty, SerializedProperty, SerializedProperty, SerializedProperty, SerializedProperty)>
        {
            { "Left", (serializedObject.FindProperty("ArrowLeft"), serializedObject.FindProperty("tagToFoundCadreLeft"), serializedObject.FindProperty("targetCadreLeft"), serializedObject.FindProperty("arrowLeft"), serializedObject.FindProperty("angleTargetLeft")) },
            { "Right", (serializedObject.FindProperty("ArrowRight"), serializedObject.FindProperty("tagToFoundCadreRight"), serializedObject.FindProperty("targetCadreRight"), serializedObject.FindProperty("arrowRight"), serializedObject.FindProperty("angleTargetRight")) },
            { "Up", (serializedObject.FindProperty("ArrowUp"), serializedObject.FindProperty("tagToFoundCadreUp"), serializedObject.FindProperty("targetCadreUp"), serializedObject.FindProperty("arrowUp"), serializedObject.FindProperty("angleTargetUp")) },
            { "Down", (serializedObject.FindProperty("ArrowDown"), serializedObject.FindProperty("tagToFoundCadreDown"), serializedObject.FindProperty("targetCadreDown"), serializedObject.FindProperty("arrowDown"), serializedObject.FindProperty("angleTargetDown")) }
        };
    }
    
    public override void OnInspectorGUI()
    {
        GestionCadre gestionCadre = (GestionCadre)target;

        if (!gestionCadre.enabled)
        {
            return;
        }
        
        serializedObject.Update();
        
        EditorGUILayout.LabelField("Property", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("player"), new GUIContent("NavMesh Agent"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("center"), new GUIContent("Center Transform"));

        EditorGUILayout.Space();

        foreach (var entry in arrowMappings)
        {
            string direction = entry.Key;
            var props = entry.Value;

            EditorGUILayout.PropertyField(props.Arrow, new GUIContent($"Arrow {direction}"));
            
            if (props.Arrow.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(props.TagTarget, new GUIContent($"Tag Target Cadre ({direction})"));
                EditorGUILayout.PropertyField(props.Target, new GUIContent($"Target Cadre ({direction})"));
                EditorGUILayout.PropertyField(props.ArrowObj, new GUIContent($"Arrow ({direction})"));
                EditorGUILayout.PropertyField(props.Angle, new GUIContent($"Angle Target ({direction})"));
                EditorGUI.indentLevel--;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
