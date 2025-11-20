using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Settings", menuName = "Status Effects/Explosion Settings")]
public class ExplosionData : ScriptableObject
{

    [Header("General Settings")]
    [SerializeField] private bool usePooling = false;


    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float explosionDamage = 50f;

    [Header("Visual & Audio Effects")]
    [SerializeField] private GameObject explosionEffectPrefab;

    public float explosionChance = 0f;
    [SerializeField] private float baseExplosionChance = 0.2f;
    [SerializeField] private float explosionChanceIncrease = 0.1f;

    public float ExplosionChanceIncrease => explosionChanceIncrease;
    public float BaseExplosionChance => baseExplosionChance;
    public float ExplosionRadius => explosionRadius;
    public float ExplosionForce => explosionForce;
    public float ExplosionDamage => explosionDamage;

    public GameObject ExplosionEffectPrefab => explosionEffectPrefab;

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

        // play explosion SFX
        AudioManager.Instance?.PlaySFX("Explosion");

        ResetExplosionChance();
    }

    private void ResetExplosionChance()
    {
        explosionChance = baseExplosionChance;
    }

    public void IncreaseExplosionChance()
    {
        explosionChance += explosionChanceIncrease;
    }

    public bool RunExplosionLogic(GameObject gameObject)
    {
        float roll = Random.Range(0f, 1f);
        if (roll <= explosionChance)
        {
            // Trigger explosion
            // Note: The actual explosion logic should be handled elsewhere, this is just a chance check
            Explode(gameObject.transform);
            return true;
        }
        else
        {
            IncreaseExplosionChance();
            return false;
        }
    }
}
