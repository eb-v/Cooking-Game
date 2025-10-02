using UnityEngine;

[CreateAssetMenu(fileName = "BaseKitchenObjectSO", menuName = "Scriptable Objects/BaseKitchenObject")]
public class BaseKitchenObjectSO : ScriptableObject {
    
    public Transform prefab;
    public Sprite sprite;
    public string objectName;

}