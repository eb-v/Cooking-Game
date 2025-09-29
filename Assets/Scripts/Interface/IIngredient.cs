using UnityEngine;

public interface IIngredient
{
    bool isPrepared { get; set; }
    GameObject preparedVersionPrefab { get; set; }
}
