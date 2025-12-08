using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Explosion System", menuName = "Systems/Explosion System")]
public class ExplosionSystem : ScriptableObject
{
    private static ExplosionSystem instance;

    public static ExplosionSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ExplosionSystem>("Systems/Explosion System");
            }
            return instance;
        }
    }

    [SerializeField] private bool systemEnabled = true;

    [Header("General Settings")]
    [SerializeField] private static bool usePooling = false;

    public static bool SystemEnabled
    {
        get { return Instance.systemEnabled; }
        set { Instance.systemEnabled = value; }
    }

    private void OnEnable()
    {
        SystemEnabled = systemEnabled;
    }

    private void OnValidate()
    {
        SystemEnabled = systemEnabled;
    }

    public static bool RunExplosionLogic(List<Transform> explosionPoints, ExplosionData explosionData)
    {
        if (!SystemEnabled)
            return false;

        float roll = Random.Range(0f, 1f);
        if (roll <= explosionData.explosionChance)
        {
            //explosion sfx

            // Trigger explosion
            // Note: The actual explosion logic should be handled elsewhere, this is just a chance check
            foreach (Transform point in explosionPoints)
            {
                Explode(point, explosionData);
            }
            ResetExplosionChance(out explosionData.explosionChance, explosionData.BaseExplosionChance);
            return true;
        }
        else
        {
            IncreaseExplosionChance(ref explosionData.explosionChance, explosionData.ExplosionChanceIncrease);
            return false;
        }
    }



    private static void Explode(Transform explosionPoint, ExplosionData explosionData)
    {
        if (usePooling)
        {
            ObjectPoolManager.SpawnObject(explosionData.ExplosionEffectPrefab, explosionPoint.position, explosionPoint.rotation);
        }
        else
        {
            GameObject explosion = Instantiate(explosionData.ExplosionEffectPrefab, explosionPoint.position, explosionPoint.rotation);
            ParticleSystem explosionPs = explosion.GetComponent<ParticleSystem>();
            float totalDuration = explosionPs.main.duration + explosionPs.main.startLifetime.constantMax;
            Destroy(explosion, totalDuration);
        }
    }

    private static void ResetExplosionChance(out float explosionChance, float baseExplosionChance)
    {
        explosionChance = baseExplosionChance;
    }

    private static void IncreaseExplosionChance(ref float explosionChance, float explosionChanceIncrease)
    {
        float newExplosionChance = explosionChance + explosionChanceIncrease;
        explosionChance = newExplosionChance;
    }
}
