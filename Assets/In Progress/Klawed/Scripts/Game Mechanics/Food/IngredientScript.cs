using UnityEngine;

public class IngredientScript : MonoBehaviour, IGrabable 
{
    [SerializeField] private Ingredient ingredient;

    public Ingredient Ingredient => ingredient;

    public bool isGrabbed { get; set; }
    [field: SerializeField] public GrabData grabData { get; set; }
    public GameObject player { get; set; }

    public void GrabObject(GameObject player)
    {
        PhysicsTransform physicsTransform = gameObject.GetComponent<PhysicsTransform>();
        GrabSystem.GrabObject(player, physicsTransform.physicsTransform.gameObject, grabData);
    }


}
