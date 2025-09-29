using UnityEngine;

public interface IPrepStation
{
    GameObject currentUser { get; set; }
    GameObject currentPlacedObject { get; set; }
    bool containsObject { get; set; }

    bool isBeingUsed { get; set; }

    // change ingredient prefab to prepared version
    void PrepIngredient();

}
