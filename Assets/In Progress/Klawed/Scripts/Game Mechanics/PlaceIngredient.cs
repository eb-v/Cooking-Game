using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlaceIngredient : MonoBehaviour
{
    [SerializeField] private Transform targetPosition;
    [SerializeField] private List<Ingredient> allowedIngredients;

    private void OnEnable()
    {
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).AddListener(OnPlayerInteract);
    }

    private void OnDisable()
    {
        GenericEvent<InteractEvent>.GetEvent(gameObject.name).RemoveListener(OnPlayerInteract);
    }


    private void OnPlayerInteract(GameObject player)
    {
        GrabScript leftGrabScript = player.GetComponent<HandContainer>().LeftHand.GetComponent<GrabScript>();
        GrabScript rightGrabScript = player.GetComponent<HandContainer>().RightHand.GetComponent<GrabScript>();
        // Check if only one hand is holding something
        if (!(leftGrabScript.isGrabbing && rightGrabScript.isGrabbing))
        {
            if (leftGrabScript.isGrabbing || rightGrabScript.isGrabbing)
            {
                if (leftGrabScript.isGrabbing)
                {
                    GameObject grabbedObject = leftGrabScript.grabbedObj;
                    IngredientScript ingredientScript = grabbedObject.GetComponent<IngredientScript>();
                    if (ingredientScript == null)
                        return;
                    if (allowedIngredients.Contains(ingredientScript.Ingredient))
                    {
                        //GrabSystem.ReleaseObject(leftGrabScript.gameObject);
                        grabbedObject.transform.position = targetPosition.position;
                        grabbedObject.transform.rotation = Quaternion.identity;
                        Debug.Log("Placed ingredient: " + ingredientScript.Ingredient.name);
                    }
                }
                else
                {
                    GameObject grabbedObject = rightGrabScript.grabbedObj;
                    IngredientScript ingredientScript = grabbedObject.GetComponent<IngredientScript>();
                    if (ingredientScript == null)
                        return;
                    if (allowedIngredients.Contains(ingredientScript.Ingredient))
                    {
                        //GrabSystem.ReleaseObject(rightGrabScript.gameObject);
                        grabbedObject.transform.position = targetPosition.position;
                        grabbedObject.transform.rotation = Quaternion.identity;
                        Debug.Log("Placed ingredient: " + ingredientScript.Ingredient.name);
                    }

                }
            }
        }
        else
        {
            // if both hands are holding something
            // check if the hands are holding the same thing
            if (leftGrabScript.grabbedObj.GetInstanceID() == rightGrabScript.grabbedObj.GetInstanceID())
            {
                // do check for only one hand since they are holding the same object
                GameObject grabbedObject = rightGrabScript.grabbedObj;
                IngredientScript ingredientScript = grabbedObject.GetComponent<IngredientScript>();
                if (ingredientScript == null)
                    return;
                if (allowedIngredients.Contains(ingredientScript.Ingredient))
                {
                    // release object from both hands
                   // GrabSystem.ReleaseObject(rightGrabScript.gameObject);
                   // GrabSystem.ReleaseObject(leftGrabScript.gameObject);
                    grabbedObject.transform.position = targetPosition.position;
                    grabbedObject.transform.rotation = Quaternion.identity;
                }
            }
            
        }
    }


}
