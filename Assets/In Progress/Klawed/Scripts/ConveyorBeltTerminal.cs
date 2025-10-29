using UnityEngine;
using System.Collections.Generic;

public class ConveyorBeltTerminal : MonoBehaviour
{
    [SerializeField] private List<ConveyorBelt> conveyorBelt;

    private void Start()
    {
        GenericEvent<DPadInteractEvent>.GetEvent(gameObject.name).AddListener(OnDpadInteract);
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(OnInteract);
    }

    private void OnDpadInteract(Vector2 input)
    {
        
        if (input.x == 1 && input.y == 0)
        {
            foreach (ConveyorBelt belt in conveyorBelt)
            {
                belt.ChangeDirection(1);
            }
        }
        else if (input.x == -1 && input.y == 0)
        {
            foreach (ConveyorBelt belt in conveyorBelt)
            {
                belt.ChangeDirection(-1);
            }   
        }
    }

    private void OnInteract(GameObject player)
    {
        foreach (ConveyorBelt belt in conveyorBelt)
        {
            belt.ChangeRunningStatus();
        }
    }


}
