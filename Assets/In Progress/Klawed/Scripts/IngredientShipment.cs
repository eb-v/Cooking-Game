using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class IngredientShipment : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] int amountToSpawn = 4;
    [SerializeField] private string _assignedChannel = "DefaultChannel";    
    private List<GameObject> _ingredientsInCrate = new List<GameObject>();
    [SerializeField] private List<GameObject> _crateParts;

    private void Start()
    {
        GenericEvent<CrateLandedEvent>.GetEvent(gameObject.name).AddListener(OnCrateLanded);
        GenericEvent<IngredientOrderedEvent>.GetEvent(_assignedChannel).AddListener(PrepareShipment);
    }

    // prepare shipment crate with disabled ingredient objects
    private void PrepareShipment(GameObject orderedIngredient)
    {
        foreach(Transform spawnPoint in spawnPoints)
        {
            GameObject ingredient = ObjectPoolManager.SpawnObject(orderedIngredient, spawnPoint.position, Quaternion.identity);
            _ingredientsInCrate.Add(ingredient);
            ingredient.transform.SetParent(transform);
            ingredient.SetActive(false);
        }
    }

    private void OnCrateLanded()
    {
        EnableIngredients();
        DisassembleCrate();
    }

    private void EnableIngredients()
    {
        foreach(GameObject ingredient in _ingredientsInCrate)
        {
            ingredient.transform.SetParent(null);
            ingredient.SetActive(true);
        }
    }

    private void DisassembleCrate()
    {
        Destroy(gameObject.GetComponent<BoxCollider>());
        foreach (GameObject cratePart in _crateParts)
        {
            //cratePart.transform.SetParent(null);
            FixedJoint joint = cratePart.GetComponent<FixedJoint>();
            cratePart.layer = LayerMask.NameToLayer("Default");
            if (joint == null) continue;
            Destroy(joint);
        }
    }


}
