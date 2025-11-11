using UnityEngine;

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
                instance = Resources.Load<ExplosionSystem>("Explosion System");
            }
            return instance;
        }
    }

    [SerializeField] private bool systemEnabled = true;

    [Header("References")]
    [SerializeField] private GameObject explosionPrefab;

    public static void Explode(GameObject objToExplode)
    {
        Instantiate(Instance.explosionPrefab, objToExplode.transform.position, Quaternion.identity);
    }

}
