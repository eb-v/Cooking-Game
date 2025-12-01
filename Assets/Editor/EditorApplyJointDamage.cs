using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LimbHP))]
public class EditorApplyJointDamage : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. Draw the default Inspector elements (like the script field)
        DrawDefaultInspector();

        // 2. Get a reference to the component instance being inspected
        LimbHP myTarget = (LimbHP)target;

        // 3. Create the actual button in the Inspector
        if (GUILayout.Button("Inflict Max Damage"))
        {
            // 4. Call the function on the target component when the button is pressed
            myTarget.ApplyDamage(myTarget.maxHP);
        }
    }
}
