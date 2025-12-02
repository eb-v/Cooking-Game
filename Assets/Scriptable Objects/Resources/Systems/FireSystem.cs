using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
    [SerializeField] private FireSettings settings;

    // (Burnable) -> (Fire GameObject)
    private HashSet<Burnable> spreadThisFrame = new HashSet<Burnable>();

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


    public void IgniteObject(ref bool isOnFire, ref float burnProgress, List<FireController> fireEffects) 
    {
        if (!SystemEnabled)
            return;
        
        isOnFire = true;
        burnProgress = 1f;

        foreach (FireController fireEffect in fireEffects)
        {
            fireEffect.StartFire();
        }
    }


    public void ExtinguishObject(Burnable objToExtinguish)
    {
        if (!SystemEnabled)
            return;
    }

    public void SpreadFire(Vector3 position, float spreadRadius)
    {
        if (!SystemEnabled)
            return;

        List<Burnable> burnablesInRange = GetFlammableObjectsInRange(position, spreadRadius);

        foreach (Burnable burnable in burnablesInRange)
        {
            if (!spreadThisFrame.Contains(burnable))
            {
                burnable.ModifyBurnProgress(Instance.settings.burnMultiplier * Time.deltaTime);
            }
        }
    }

   

    private static List<Burnable> GetFlammableObjectsInRange(Vector3 pos, float spreadRadius)
    {
        Collider[] hits = Physics.OverlapSphere(pos, spreadRadius);
        List<Burnable> flammable = new List<Burnable>();

        foreach (var hit in hits)
        {
            GameObject rootObj = hit.gameObject;
            if (rootObj.TryGetComponent(out Burnable flammableComponent))
            {
                if (!flammable.Contains(flammableComponent))
                    flammable.Add(flammableComponent);
            }
        }

        return flammable;
    }

    public void EndFrame()
    {
        spreadThisFrame.Clear();
    }

}
