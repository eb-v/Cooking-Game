using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TrainManager))]
public class EditorLaunchTrain : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Launch Train"))
        {
            TrainManager.Instance.LaunchTrain();
        }
    }
}
