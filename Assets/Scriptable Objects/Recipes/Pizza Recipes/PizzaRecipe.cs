using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PizzaRecipe", menuName = "Scriptable Objects/Data/Recipes/Pizza")]
public class PizzaRecipe : ScriptableObject
{
    public List<Ingredient> requiredToppings;
    public MenuItem resultingPizza;
}
