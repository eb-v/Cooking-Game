using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void Interact(GameObject player)
    {
        if (this.enabled)
            GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).Invoke(player);
    }

    public void AltInteract(GameObject player)
    {
        if (this.enabled)
            GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).Invoke(player);
    }

}
