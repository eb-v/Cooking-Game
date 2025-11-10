using System.Collections.Generic;
using UnityEngine;

public class AssembledItemObject : MonoBehaviour
{
    private List<GameObject> PreparedIngredients;

    public AssembledItemObject()
    {
        PreparedIngredients = new List<GameObject>();
    }

    public void AddIngredient(GameObject PreparedIngredient)
    {
        PreparedIngredients.Add(PreparedIngredient);
    }

    public List<GameObject> GetIngredients()
    {
        return PreparedIngredients;
    }

    public void ClearIngredients(){
        PreparedIngredients.Clear();
    }
}
