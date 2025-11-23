using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fire System", menuName = "Systems/Fire System")]
public class FireSystem : ScriptableObject
{
    private static FireSystem instance;

    public static FireSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<FireSystem>("Systems/Fire System");
            }
            return instance;
        }
    }

    [SerializeField] private bool systemEnabled = true;


    [Header("References")]
    [SerializeField] private GameObject firePrefab;

    [Header("Settings")]
    [SerializeField] private BurnableSettings settings;

    // (IFlammable) -> (Fire GameObject)
    private static Dictionary<IFlammable, List<FireController>> ignitedObjects = new Dictionary<IFlammable, List<FireController>>();

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


    public static void IgniteObject(IFlammable objToIgnite) {
        if (!SystemEnabled)
            return;

        if (objToIgnite.IsOnFire)
            return;

        foreach (FireController fireEffect in objToIgnite.FireEffects)
        {
            fireEffect.StartFire();
        }
        objToIgnite.IsOnFire = true;
    }


    public static void ExtinguishObject(IFlammable objToExtinguish)
    {
        if (!SystemEnabled)
            return;

        if (!objToExtinguish.IsOnFire)
            return;

        foreach (FireController fireController in objToExtinguish.FireEffects)
        {
            fireController.StopFire();
        }
        objToExtinguish.IsOnFire = false;
    }

    public void SpreadFire(Vector3 position, float spreadRadius)
    {
        if (!SystemEnabled)
            return;

        List<IFlammable> flammables = GetFlammableObjectsInRange(position, spreadRadius);
        foreach (IFlammable flammable in flammables)
        {
            if (!flammable.IsOnFire)
            {
                if (settings == null)
                {
                    Debug.LogError("FireSystem SpreadFire: settings is null!");
                    return;
                }
                    
                flammable.ModifyBurnProgress(settings.spreadMultiplier * Time.deltaTime);
            }
        }

    }

    private static List<IFlammable> GetFlammableObjectsInRange(Vector3 pos, float spreadRadius)
    {
        Collider[] hits = Physics.OverlapSphere(pos, spreadRadius);
        List<IFlammable> flammable = new List<IFlammable>();

        foreach (var hit in hits)
        {
            GameObject rootObj = hit.transform.root.gameObject;
            if (rootObj.TryGetComponent(out IFlammable flammableComponent))
            {
                if (!flammable.Contains(flammableComponent))
                    flammable.Add(flammableComponent);
            }
        }

        return flammable;
    }

}
