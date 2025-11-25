using UnityEngine;
using System.Collections.Generic;

public class ConveyorBeltTerminal : MonoBehaviour
{
    [SerializeField] private List<ConveyorBelt> conveyorBelt;
    [SerializeField] private ExplosionData explodeSettings;
    private float explosionChance = 0.1f; // 10% chance to explode on interaction

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

    private void RunExplosionChance()
    {
        if (Random.value < explosionChance)
        {
            Explode();
        }
        else
        {
            explosionChance += 0.05f;
        }
    }

    private void Explode()
    {
    }
}
