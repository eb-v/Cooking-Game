using UnityEngine;

public class IngredientScript : MonoBehaviour, IInteractable, IGrabable 
{
    [SerializeField] private Ingredient ingredient;

    public Ingredient Ingredient => ingredient;

    public bool isGrabbed { get; set; }

    private void Awake()
    {
        GenericEvent<OnInteractInput>.GetEvent(gameObject.name).AddListener(OnInteract);
        GenericEvent<OnAlternateInteractInput>.GetEvent(gameObject.name).AddListener(OnAlternateInteract);
    }

    public void OnInteract(GameObject player)
    {
        if (!isGrabbed)
        {
            isGrabbed = true;
            GrabSystem.GrabObject(player, gameObject);
        }
    }

    public void OnAlternateInteract(GameObject player)
    {
        if (isGrabbed)
        {
            isGrabbed = false;
            GrabSystem.ReleaseObject(player, gameObject);
        }
    }

    



}
