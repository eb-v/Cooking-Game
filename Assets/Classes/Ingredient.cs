using System.Runtime.Serialization;
using UnityEngine;





public class Ingredient
{
    public GameObject gameObject { get; set; }
    private bool isPrepared { get; set; }
    public GameObject preparedIngredientPrefab { get; set; }

    public Ingredient(GameObject gameObject, GameObject preparedIngredientPrefab)
    {
        this.gameObject = gameObject;
        this.preparedIngredientPrefab = preparedIngredientPrefab;
        isPrepared = false;
    }

    public void SpawnPreparedVersion()
    {
        GameObject preparedIngredient = ObjectPoolManager.SpawnObject(preparedIngredientPrefab, gameObject.transform.position, Quaternion.identity);
        Rigidbody rb = preparedIngredient.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        Vector3 popDirection = Vector3.up;
        rb.AddForce(popDirection * 8f, ForceMode.Impulse);


        DestroySelf();
    }

    public bool IsPrepared()
    {
        return isPrepared;
    }

    public void DestroySelf()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

}
