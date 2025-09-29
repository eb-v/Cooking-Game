using UnityEngine;

public class TomatoScript : MonoBehaviour, IIngredient
{
    public bool isPrepared { get; set; }
    public GameObject preparedVersionPrefab { get; set; }
}
