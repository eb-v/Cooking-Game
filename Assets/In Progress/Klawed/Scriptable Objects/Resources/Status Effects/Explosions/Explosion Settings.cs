using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Settings", menuName = "Status Effects/Explosion Settings")]
public class ExplosionSettings : ScriptableObject
{

    [Header("General Settings")]
    [SerializeField] private bool usePooling = false;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float explosionDamage = 50f;

    [Header("Visual & Audio Effects")]
    [SerializeField] private GameObject explosionEffectPrefab;

    public float ExplosionRadius => explosionRadius;
    public float ExplosionForce => explosionForce;
    public float ExplosionDamage => explosionDamage;

    public void Explode(Transform objTransform)
    {
        if (usePooling)
        {
            ObjectPoolManager.SpawnObject(explosionEffectPrefab, objTransform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(explosionEffectPrefab, objTransform.position, Quaternion.identity);
        }
    }
}
