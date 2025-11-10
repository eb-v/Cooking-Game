using UnityEngine;

[CreateAssetMenu(fileName = "UIAnimSequenceSO", menuName = "Scriptable Objects/UI/UIAnimSequenceSO")]
public class UIAnimSequenceSO : ScriptableObject {
    [Tooltip("Delay before starting the next spring sequence after this one finishes.")]
    public float endDelay = 0f;
}