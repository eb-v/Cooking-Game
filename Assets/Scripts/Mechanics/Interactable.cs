using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void Interact(GameObject player)
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).Invoke(player);
    }

    public void AltInteract(GameObject player)
    {
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).Invoke(player);
    }

}
