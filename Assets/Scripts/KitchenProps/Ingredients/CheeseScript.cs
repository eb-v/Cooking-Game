using UnityEngine;

public class CheeseScript : MonoBehaviour, IIngredient
{
    public bool isPrepared { get; set; }

    public GameObject preparedIngredientPrefab { get; set; }

    public void SwitchIngredientPrefab(GameObject newPrefab)
    {
        
    }

    public void DestroySelf()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

}
