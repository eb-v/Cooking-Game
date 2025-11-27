using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Grabable))]
public class IngredientScript : MonoBehaviour
{
    [SerializeField] private Ingredient ingredient;

    public Ingredient Ingredient => ingredient;

    [field: SerializeField] public List<BaseRecipe> recipes { get; set; }

    //public override void OnInteract(GameObject player)
    //{
    //    base.OnInteract(player);
    //}

    //public override void GrabObject(GameObject player)
    //{
    //    base.GrabObject(player);
    //}
    //public override void ReleaseObject(GameObject player)
    //{
    //    base.ReleaseObject(player);
    //}

    //public override void ThrowObject(GameObject player, float throwForce)
    //{
    //    base.ThrowObject(player, throwForce);
    //}
}
