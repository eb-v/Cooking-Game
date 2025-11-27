using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ScoreCounterUIScript))]
public class ScoreUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. Draw the default Inspector elements (like the script field)
        DrawDefaultInspector();

        // 2. Get a reference to the component instance being inspected
        ScoreCounterUIScript myTarget = (ScoreCounterUIScript)target;

        // 3. Create the actual button in the Inspector
        if (GUILayout.Button("Set Random Score"))
        {
            // 4. Call the function on the target component when the button is pressed
            myTarget.DebugSpin();
        }

        if (GUILayout.Button("Set Spring Values"))
        {
            myTarget.SetSpringValues();
        }
    }
}
