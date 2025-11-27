using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveGrabData))]
public class SaveGrabDataEditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. Draw the default Inspector elements (like the script field)
        DrawDefaultInspector();

        // 2. Get a reference to the component instance being inspected
        SaveGrabData myTarget = (SaveGrabData)target;

        // 3. Create the actual button in the Inspector
        if (GUILayout.Button("Save Grab Data"))
        {
            // 4. Call the function on the target component when the button is pressed
            myTarget.SaveGrabDataFunction();
        }
    }
}
