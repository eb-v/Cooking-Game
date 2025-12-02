using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlaceIngredient : MonoBehaviour
{
    [SerializeField] private Transform targetPosition;
    [SerializeField] private List<Ingredient> allowedIngredients;

    private void OnEnable()
    {
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(OnPlayerInteract);
    }

    private void OnDisable()
    {
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).RemoveListener(OnPlayerInteract);
    }


    private void OnPlayerInteract(GameObject player)
    {
       
    }


}
