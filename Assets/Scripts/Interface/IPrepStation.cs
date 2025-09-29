using UnityEngine;

public interface IPrepStation
{
    GameObject currentPlacedObject { get; set; }
    bool containsObject { get; set; }

    // change ingredient prefab to prepared version
    void PrepIngredient(GameObject ingredient);

}
