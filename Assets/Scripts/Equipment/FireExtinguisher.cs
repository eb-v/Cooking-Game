using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Grabable))]
public class FireExtinguisher : Equipment
{
    [SerializeField] private GameObject foamPrefab;
    [SerializeField] private Transform spawnTransform;

    [Header("References")]
    [SerializeField] private ConeGizmo cone;

    [Header("Settings")]
    [SerializeField] private float minFoamScale = 0.2f;
    [SerializeField] private float maxFoamScale = 0.4f;
    [SerializeField] private float offsetRange = 0.1f;
    [SerializeField] private float minForce = 800f;
    [SerializeField] private float maxForce = 1200f;


    public override void UseEquipment()
    {
        SprayFoam();
        SprayFoam();
        SprayFoam();

        //ExtinguishFlamablesInRange();
    }

    private void SprayFoam()
    {
       GameObject foamParticle = ObjectPoolManager.SpawnObject(foamPrefab, spawnTransform.position, spawnTransform.rotation);
        Vector3 foamScale = Vector3.one;
        CalculateRandomScale(ref foamScale);
        foamParticle.transform.localScale = foamScale;
        LaunchFoam(foamParticle);
    }

    private void LaunchFoam(GameObject foamParticle)
    {
        Rigidbody foamRigidbody = foamParticle.GetComponentInChildren<Rigidbody>();
        if (foamRigidbody != null)
        {
            Vector3 direction = -spawnTransform.forward;
            CalculateRandomOffset(ref direction);
            float force = 800f;
            CalculateRandomForce(ref force);
            foamRigidbody.AddForce(direction * force); // Adjust force as needed
        }
    }

    private void CalculateRandomOffset(ref Vector3 direction)
    {
        direction.x += Random.Range(-offsetRange, offsetRange);
        direction.y += Random.Range(-offsetRange, offsetRange);
        direction.z += Random.Range(-offsetRange, offsetRange);
        direction.Normalize();
    }

    private void CalculateRandomForce(ref float force)
    {
        force = Random.Range(minForce, maxForce);
    }

    private void CalculateRandomScale(ref Vector3 scale)
    {
        float randomScale = Random.Range(minFoamScale, maxFoamScale);
        scale = new Vector3(randomScale, randomScale, randomScale);
    }

    //private void ExtinguishFlamablesInRange()
    //{

    //    List<GameObject> objectsInSphere = cone.GetObjectsInSphere();
    //    foreach (GameObject obj in objectsInSphere)
    //    {
    //        Burnable flammable = obj.GetComponentInChildren<Burnable>();
    //        if (flammable != null && flammable.IsOnFire)
    //        {
    //            flammable.Extinguish();
    //            Debug.Log($"[FIRE EXTINGUISHER] Extinguished fire on object: {obj.name}");
    //        }
    //    }

    //}


    //private void ExtinguishFire(IFlammable flammable)
    //{
    //    flammable.Extinguish();
    //}

}
