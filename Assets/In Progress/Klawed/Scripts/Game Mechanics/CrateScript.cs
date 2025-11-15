using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CrateScript : MonoBehaviour, IDespawnable
{
    [SerializeField] private float explosionForce = 600f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private int ingredientCount = 4;
    [SerializeField] private float despawnDelay = 10f;

    private List<GameObject> crateParts = new List<GameObject>();

    private bool isDisassembled = false;

    private void Start()
    {
        foreach (Transform part in transform)
        {
            crateParts.Add(part.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && !isDisassembled)
        {
            SpawnIngredients();
            DisassembleCrate();
        }
    }
    // apply an explosive force from the center point of the crate outwards
    private void DisassembleCrate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Destroy(rb);
        foreach (GameObject part in crateParts)
        {
            if (part.GetComponent<Rigidbody>() == null)
                part.AddComponent<Rigidbody>();
        }
        ApplyForce();
        isDisassembled = true;
        Despawn();
    }

    private void ApplyForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }
    
    public void SetIngredient(GameObject ingredient)
    {
        ingredientPrefab = ingredient;
    }

    private void SpawnIngredients()
    {
        for (int i = 0; i < ingredientCount; i++)
        {
            ObjectPoolManager.SpawnObject(ingredientPrefab, transform.position, Quaternion.identity);
        }
    }

    public void ResetValues()
    {
        
    }

    public void Despawn()
    {
        StartCoroutine(DespawnCoroutine(despawnDelay));
    }

    public IEnumerator DespawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetValues();
        Destroy(gameObject);
    }
}
