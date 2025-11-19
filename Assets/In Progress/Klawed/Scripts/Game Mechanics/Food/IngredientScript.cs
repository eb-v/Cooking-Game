using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class IngredientScript : MonoBehaviour, IGrabable, IInteractable
{
    [SerializeField] private Ingredient ingredient;

    public Ingredient Ingredient => ingredient;

    public bool isGrabbed { get; set; }
    [field: SerializeField] public GrabData grabData { get; set; }
    [field: SerializeField] public GameObject currentPlayer { get; set; }

    [field: SerializeField] public List<BaseRecipe> recipes { get; set; }


    public void OnInteract(GameObject player)
    {
        GrabObject(player);
    }

    public void GrabObject(GameObject player)
    {
        GrabScript gs = player.GetComponent<GrabScript>();
        if (gs.isGrabbing)
            return;

        if (isGrabbed)
        {
            GrabScript playerGrabScript = currentPlayer.GetComponent<GrabScript>();
            playerGrabScript.grabbedObject = null;
            ReleaseObject(currentPlayer);
        }

        PhysicsTransform physicsTransform = gameObject.GetComponent<PhysicsTransform>();
        GrabSystem.GrabObject(player, physicsTransform.physicsTransform.gameObject, grabData);
        GenericEvent<OnObjectGrabbed>.GetEvent(player.name).Invoke(this);

        isGrabbed = true;
        currentPlayer = player;
    }

    public void ReleaseObject(GameObject player)
    {
        GrabSystem.ReleaseObject(player);
        isGrabbed = false;
        currentPlayer = null;
    }

    public GameObject GetGameObject() => gameObject;
}
