using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;


public class OrderDisplayScript : MonoBehaviour
{
    [SerializeField] private string _assignedChannel;

    public bool useAssignedChannel = true;



    private void Awake()
    {
        if (useAssignedChannel)
        {
            GenericEvent<NewOrderAddedEvent>.GetEvent(_assignedChannel).AddListener(OnNewOrderAdded);
            GenericEvent<OrderCompletedEvent>.GetEvent(_assignedChannel).AddListener(OnOrderCompleted);
            GenericEvent<MaxOrderSetEvent>.GetEvent(_assignedChannel).AddListener(OnMaxOrderSet);
        }
        else
        {
            GenericEvent<NewOrderAddedEvent>.GetEvent(gameObject.name).AddListener(OnNewOrderAdded);
            GenericEvent<OrderCompletedEvent>.GetEvent(gameObject.name).AddListener(OnOrderCompleted);
            GenericEvent<MaxOrderSetEvent>.GetEvent(gameObject.name).AddListener(OnMaxOrderSet);
        }
    }


    private void OnNewOrderAdded(MenuItem newOrder)
    {
        // Update the UI to display the new order
        UpdateOrderVisuals();
    }

    private void OnMaxOrderSet(int maxOrders)
    {
    }


    private void OnOrderCompleted(MenuItem completedOrder)
    {
       

        UpdateOrderVisuals(); 
    }

    private void UpdateOrderVisuals()
    {
        
    }



}
