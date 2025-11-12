using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance { get; private set; }
    private int deliveredCount = 0;
    public UnityEvent<int> OnDishDelivered;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        if (OnDishDelivered == null) {
            OnDishDelivered = new UnityEvent<int>();
        }
    }

    public void AddDeliveredDish(int ingredientCount)
    {
        deliveredCount++;
        Debug.Log("Total delivered: " + deliveredCount + " | Ingredients: " + ingredientCount);
        // Invoke with ingredient count instead of total delivered count
        OnDishDelivered.Invoke(ingredientCount);
    }

    public void ResetPoints()
    {
        deliveredCount = 0; // or whatever your point variable is named
        OnDishDelivered?.Invoke(0); // This will update all UIs listening to it
    }

    public int GetDeliveredCount()
    {
        return deliveredCount;
    }
}