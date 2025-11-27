using UnityEditor;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerData playerSettings;
    [SerializeField] private Transform pelvis;
    [SerializeField] private LayerMask interactLayerMask;


    private void OnEnable()
    {
        GenericEvent<OnInteractInput>.GetEvent(gameObject.name).AddListener(HandleInteract);
        GenericEvent<OnAlternateInteractInput>.GetEvent(gameObject.name).AddListener(HandleAltInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractInput>.GetEvent(gameObject.name).RemoveListener(HandleInteract);
        GenericEvent<OnAlternateInteractInput>.GetEvent(gameObject.name).RemoveListener(HandleAltInteract);
    }

    private void HandleInteract(GameObject player)
    {
        Interactable interactable = InteractRayCastDetection();
        if (interactable != null)
        {
            AudioManager.Instance?.PlaySFX("Item Interact");
            interactable.Interact(player);
        }
    }

    private void HandleAltInteract(GameObject player)
    {
        Interactable interactable = InteractRayCastDetection();
        if (interactable != null)
        {
            AudioManager.Instance?.PlaySFX("Item Interact");
            interactable.AltInteract(player);
        }

    }

    private Interactable InteractRayCastDetection()
    {
        Ray ray = new Ray(pelvis.position, -pelvis.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, playerSettings.InteractionRange, interactLayerMask))
        {
            Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
            return interactable;
        }
        else
        {
            return null;
        }
    }
}
