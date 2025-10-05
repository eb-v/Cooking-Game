using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssembledItemObject{
    
    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake() {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public void AddIngredient(KitchenObjectSO kitchenObjectSO) {
            kitchenObjectSOList.Add(kitchenObjectSO);
    }
}