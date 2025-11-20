using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Env_Interaction env_Interaction;
    private GameObject lookedAtObj => env_Interaction.currentlyLookingAt;

    private void Start()
    {
        GenericEvent<OnInteractInput>.GetEvent(gameObject.name).AddListener(HandleInteract);
        GenericEvent<OnAlternateInteractInput>.GetEvent(gameObject.name).AddListener(HandleAltInteract);
    }

    private void HandleInteract(GameObject player)
    {
        if (lookedAtObj == null) return;

        IInteractable interactable = lookedAtObj.GetComponent<IInteractable>();
        if (interactable == null) return;

        // play interact SFX
        AudioManager.Instance?.PlaySFX("Item Interact");

        interactable.OnInteract(player);
    }

    private void HandleAltInteract(GameObject player)
    {
        if (lookedAtObj == null) return;

        IAltInteractable altInteractable = lookedAtObj.GetComponent<IAltInteractable>();
        if (altInteractable == null) return;

        // use same SFX
        AudioManager.Instance?.PlaySFX("Item Interact");

        altInteractable.OnAltInteract(player);
    }
}
