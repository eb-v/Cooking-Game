using UnityEngine;

public class FireExtinguisher : DynamicObjectBase, IEquipment
{
    [SerializeField] private GameObject foamPrefab;
    [SerializeField] private Transform spawnTransform;

    [Header("Foam Settings")]
    [SerializeField] private float minFoamScale = 0.2f;
    [SerializeField] private float maxFoamScale = 0.4f;
    [SerializeField] private float offsetRange = 0.1f;

    public void UseEquipment()
    {
        SprayFoam();
        SprayFoam();
        SprayFoam();
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
        float minForce = 800f; // Minimum force
        float maxForce = 1200f; // Maximum force
        force = Random.Range(minForce, maxForce);
    }

    private void CalculateRandomScale(ref Vector3 scale)
    {
        float randomScale = Random.Range(minFoamScale, maxFoamScale);
        scale = new Vector3(randomScale, randomScale, randomScale);
    }

}
