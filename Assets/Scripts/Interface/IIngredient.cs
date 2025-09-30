using UnityEngine;

public interface IIngredient
{
    bool isPrepared { get; set; }
    GameObject preparedIngredientPrefab { get; set; }

    void SwitchIngredientPrefab(GameObject newPrefab);

    void DestroySelf();

}
