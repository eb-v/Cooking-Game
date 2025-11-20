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

        // play points SFX per successful dish
        AudioManager.Instance?.PlaySFX("Points");

        OnDishDelivered.Invoke(ingredientCount);
    }

    public void ResetPoints()
    {
        deliveredCount = 0;
        OnDishDelivered?.Invoke(0);
    }

    public int GetDeliveredCount()
    {
        return deliveredCount;
    }
}
