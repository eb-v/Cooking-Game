using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//make into event if needed
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
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //GenericEvent<DeliveredDishEvent>.GetEvent(gameObject.name).AddListener(AddDeliveredDish);

        if (OnDishDelivered == null) {
            OnDishDelivered = new UnityEvent<int>();
        }
    }

    public void AddDeliveredDish()
    {
        deliveredCount++;
        Debug.Log("Total delivered: " + deliveredCount);
        //trigger ui, sfx, animations, etc.

        OnDishDelivered.Invoke(deliveredCount);
    }

    public int GetDeliveredCount()
    {
        return deliveredCount;
    }
}
