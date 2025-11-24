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

        // play interact SFX
        AudioManager.Instance?.PlaySFX("Item Interact");

        if (lookedAtObj.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.OnInteract(player);
        }
        else
        {
            Debug.Log("could not interact with " + lookedAtObj.name);   
        }

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
