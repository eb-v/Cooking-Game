using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeathScript))]
public class EditorDeathButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DeathScript deathScript = (DeathScript)target;
        if (GUILayout.Button("Kill Player"))
        {
            deathScript.Die();
        }
    }
}
