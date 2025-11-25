// Editor/ReadOnlyPropertyDrawer.cs
using UnityEngine;
using UnityEditor;


public class ReadOnlyAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool previous = GUI.enabled;
        GUI.enabled = false;  // disable editing
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = previous;  // restore GUI state
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
