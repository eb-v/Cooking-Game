using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaptureRenderTexture))]
public class CaptureRenderTextureEditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. Draw the default Inspector elements (like the script field)
        DrawDefaultInspector();

        // 2. Get a reference to the component instance being inspected
        CaptureRenderTexture myTarget = (CaptureRenderTexture)target;

        // 3. Create the actual button in the Inspector
        if (GUILayout.Button("Capture"))
        {
            // 4. Call the function on the target component when the button is pressed
            myTarget.CaptureAndDisplay();
        }
    }
}
