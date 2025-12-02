using UnityEngine;
using System.Collections.Generic;

public class BaseRecipe : ScriptableObject
{
    public Ingredient input;
    public List<Ingredient> output;
}
