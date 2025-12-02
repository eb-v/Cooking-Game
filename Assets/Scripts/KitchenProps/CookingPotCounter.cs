using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Interactable))]
public class CookingPotCounter : MonoBehaviour
{
    [SerializeField] private Transform potSnapPoint;
    [SerializeField] private Ingredient product;
    [SerializeField] private Ingredient validIngredient;
    [ReadOnly]
    [SerializeField] private CookingPot cookingPot;
    [ReadOnly]
    [SerializeField] private float currentProgress = 0f;

    [Header("Cooking Settings")]
    [SerializeField] private float cookingDuration = 5f;


    [Header("Visual Indicators")]
    [SerializeField] private Image barImage;
    [SerializeField] private Image cookingProgressImage;


    private bool hasCookingPot = false;
    private bool isCooking = false;


    private void OnEnable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnAltInteract);
    }

    private void OnDisable()
    {
        GenericEvent<OnInteractableInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnInteract);
        GenericEvent<OnInteractableAltInteracted>.GetEvent(gameObject.GetInstanceID().ToString()).RemoveListener(OnAltInteract);
    }

    private void OnInteract(GameObject player)
    {
        GrabScript grabScript = player.GetComponent<GrabScript>();

        if (PlayerIsHoldingPot(player))
        {
            PlaceCookingPotOntoStove(grabScript.grabbedObject);
        }
    }

    private void OnAltInteract(GameObject player)
    {
        GrabScript grabScript = player.GetComponent<GrabScript>();

        if (!grabScript.IsGrabbing)
        {
            if (hasCookingPot && !isCooking)
            {
                cookingPot.EnableInteractCollider();
                Grabable potGrabableComponent = cookingPot.GetComponent<Grabable>();
                Rigidbody potRigidBody = potGrabableComponent.GetComponent<Rigidbody>();
                potRigidBody.isKinematic = false;
                potGrabableComponent.grabCollider.enabled = true;
                potGrabableComponent.Grab(player);
                hasCookingPot = false;
                cookingPot = null;
            }
            else
            {
                Debug.Log("has cooking pot false");
            }
        }
        else
        {
            Debug.Log("player is already grabbing something");
        }
    }


    private void Update()
    {
        if (isCooking)
        {
            RunCookingLogic();
        }
        else
        {
            if (CanStartCooking())
            {
                StartCooking();
            }
        }
    }

    private bool PlayerIsHoldingPot(GameObject player)
    {
        GrabScript grabScript = player.GetComponent<GrabScript>();

        if (grabScript.IsGrabbing)
        {
            Grabable grabbedObject = grabScript.grabbedObject;
            if (grabbedObject.GetComponent<CookingPot>() != null)
            {
                return true;
            }
        }
        return false;
    }

    private void PlaceCookingPotOntoStove(Grabable cookingPotGrabComponent)
    {
        cookingPotGrabComponent.Release();

        Transform potTransform = cookingPotGrabComponent.transform;
        Rigidbody potRigidbody = cookingPotGrabComponent.GetComponent<Rigidbody>();
        potRigidbody.isKinematic = true;
        potTransform.position = potSnapPoint.position;
        potTransform.rotation = potSnapPoint.rotation;

        // disable grab and interact colliders
        cookingPotGrabComponent.grabCollider.enabled = false;
        cookingPot = cookingPotGrabComponent.GetComponent<CookingPot>();
        cookingPot.DisableInteractCollider();
        hasCookingPot = true;
    }



    private void RunCookingLogic()
    {
        currentProgress += Time.deltaTime;
        UpdateVisualProgressBar(currentProgress);
        if (IsCookingComplete())
        {
            StopCooking();
            cookingPot.DestroyIngredientInPot();
            cookingPot.AddProduct(product);
        }

    }

    private void StartCooking()
    {
        isCooking = true;
        TurnOnVisuals();
        cookingProgressImage.enabled = true;
    }

    private void StopCooking()
    {
        isCooking = false;
        TurnOffVisuals();
        UpdateVisualProgressBar(0f);
    }

    private bool IsCookingComplete()
    {
        return currentProgress >= cookingDuration;
    }

    private void UpdateVisualProgressBar(float currentProgress)
    {
        cookingProgressImage.fillAmount = currentProgress / cookingDuration;
    }

    private bool CanStartCooking()
    {
        if (hasCookingPot)
        {
            if (cookingPot.HasIngredient)
            {
                Ingredient potIngredient = cookingPot.GetCurrentObject().GetComponent<IngredientScript>().Ingredient;
                if (potIngredient == validIngredient)
                {
                    return true;
                }
            }
        }

        return false;
    }


    private void TurnOnVisuals()
    {
        barImage.enabled = true;
        cookingProgressImage.enabled = true;
    }

    private void TurnOffVisuals()
    {
        barImage.enabled = false;
        cookingProgressImage.enabled = false;
    }

}