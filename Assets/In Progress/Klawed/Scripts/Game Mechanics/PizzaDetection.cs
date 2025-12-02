using UnityEngine;

public class PizzaDetection : MonoBehaviour
{
    [SerializeField] private string _assignedChannel = "DefaultChannel";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PizzaDoughBase>() != null)
        {
            GenericEvent<PizzaDoughEnteredOvenEvent>.GetEvent(_assignedChannel).Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PizzaDoughBase>() != null)
        {
            GenericEvent<PizzaDoughExitedOvenEvent>.GetEvent(_assignedChannel).Invoke(other.gameObject);
        }
    }
}
