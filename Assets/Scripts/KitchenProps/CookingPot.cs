using UnityEngine;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Grabable))]
public class CookingPot : MonoBehaviour
{
    [SerializeField] private Ingredient validIngredient;
    [SerializeField] private Transform ingredientSnapPoint;
    [SerializeField] private GameObject fillVisual;
    [SerializeField] private GameObject interactColliderObject;

    [ReadOnly]
    [SerializeField] private bool hasIngredient = false;
    public bool HasIngredient => hasIngredient;
    [ReadOnly]
    [SerializeField] private GameObject currentObject;
    [ReadOnly]
    [SerializeField] private Ingredient currentProduct;
    [ReadOnly]
    [SerializeField] private bool hasProduct = false;



    private void OnEnable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnAltInteract);
        GenericEvent<OnObjectGrabbed>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(WhenGrabbed);
        GenericEvent<OnObjectThrown>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(WhenThrown);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnAltInteract);
        GenericEvent<OnObjectGrabbed>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(WhenGrabbed);
        GenericEvent<OnObjectThrown>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(WhenThrown);
    }


    private void OnInteract(GameObject player)
    {
        Grabable heldObject = player.GetComponent<GrabScript>().grabbedObject;
        if (heldObject != null)
        {
            if (TryAddToPot(heldObject))
            {
                Debug.Log("Added ingredient to pot: " + validIngredient.IngredientName);
            }
            else
            {
                Debug.Log("Addition failed");
            }
        }
    }

    private void OnAltInteract(GameObject player)
    {
        Grabable heldObject = player.GetComponent<GrabScript>().grabbedObject;

        if (TryRemoveFromPot(player))
        {
            Debug.Log("Removed ingredient from pot: " + validIngredient.IngredientName);
        }
        else
        {
            Debug.Log("Removal failed");
        }
    }



    private bool IsValidIngredient(Grabable grabable)
    {
        if (grabable.TryGetComponent<IngredientScript>(out IngredientScript ingredientScript))
        {
            if (ingredientScript.Ingredient == validIngredient)
            {
                return true;
            }
        }
        return false;
    }


    private bool TryAddToPot(Grabable heldObject)
    {
        if (!hasIngredient && !hasProduct)
        {
            if (IsValidIngredient(heldObject))
            {
                heldObject.Release();
                heldObject.transform.position = ingredientSnapPoint.position; 
                heldObject.transform.rotation = ingredientSnapPoint.rotation;
                hasIngredient = true;
                currentObject = heldObject.gameObject;
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                heldObject.grabCollider.enabled = false;
                rb.isKinematic = true;

                heldObject.transform.Find("PhysicsCollider").gameObject.SetActive(false);
                heldObject.transform.SetParent(ingredientSnapPoint);

                return true;
            }
        }

        return false;
    }

    private bool TryRemoveFromPot(GameObject player)
    {
        if (hasIngredient)
        {
            currentObject.GetComponent<Rigidbody>().isKinematic = false;

            Grabable grabable = currentObject.GetComponent<Grabable>();
            grabable.Grab(player);
            currentObject.transform.Find("PhysicsCollider").gameObject.SetActive(true);
            hasIngredient = false;
            currentObject = null;
            return true;
        }
        return false;
    }

    public void DestroyIngredientInPot()
    {
        if (hasIngredient)
        {
            Destroy(currentObject);
            currentObject = null;
            hasIngredient = false;
        }
    }

    public void AddProduct(Ingredient product)
    {
        EnableFillVisual();
        currentProduct = product;
        hasProduct = true;
    }

    public GameObject GetCurrentObject()
    {
        return currentObject;
    }

    private void WhenGrabbed()
    {
        DisableInteractCollider();
    }

    private void WhenThrown()
    {
        EnableInteractCollider();
    }

    public void EnableFillVisual()
    {
        fillVisual.SetActive(true);
    }

    private void DisableFillVisual()
    {
        fillVisual.SetActive(false);
    }

    public void DisableInteractCollider()
    {
        interactColliderObject.SetActive(false);
    }

    public void EnableInteractCollider()
    {
        interactColliderObject.SetActive(true);
    }
}
