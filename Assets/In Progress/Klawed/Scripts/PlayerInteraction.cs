using UnityEditor;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerData playerSettings;
    [SerializeField] private Transform pelvis;
    [SerializeField] private LayerMask interactLayerMask;


    private void OnEnable()
    {
        GenericEvent<OnInteractInput>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(HandleInteract);
        GenericEvent<OnAlternateInteractInput>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(HandleAltInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractInput>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(HandleInteract);
        GenericEvent<OnAlternateInteractInput>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(HandleAltInteract);
    }

    private void HandleInteract()
    {

        Interactable interactable = InteractRayCastDetection();
        if (interactable != null)
        {
            AudioManager.Instance?.PlaySFX("Item Interact");
            interactable.Interact(gameObject);
        }
    }

    private void HandleAltInteract()
    {

        Interactable interactable = InteractRayCastDetection();
        if (interactable != null)
        {
            AudioManager.Instance?.PlaySFX("Item Interact");
            interactable.AltInteract(gameObject);
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
