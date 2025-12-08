using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    [SerializeField] private bool SystemEnabled = true;

    private static FireManager instance;

    public static FireManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<FireManager>();
            }
            return instance;
        }
    }

    [Header("References")]
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private FireSettings settings;


    private List<Burnable> objectsBurning = new List<Burnable>();



    public void IgniteObject(ref bool isOnFire, ref float burnProgress, List<FireController> fireEffects, in float spreadRadius, in Vector3 position, bool shouldSpread = true)
    {
        if (!SystemEnabled)
            return;

        isOnFire = true;
        burnProgress = 1f;

        foreach (FireController fireEffect in fireEffects)
        {
            fireEffect.StartFire();
        }
        if (shouldSpread)
        {
            // Ignite other objects within spread radius
            SpreadFire(position, spreadRadius);
        }
        
    }

    public void ExtinguishObject(ref bool isOnFire, ref float burnProgress, List<FireController> fireEffects)
    {
        if (!SystemEnabled)
            return;

        isOnFire = false;
        burnProgress = 0f;
        foreach (FireController fireEffect in fireEffects)
        {
            fireEffect.StopFire();
        }

    }

    


    public void RegisterBurningObject(Burnable burningObject)
    {
        if (!objectsBurning.Contains(burningObject))
        {
            objectsBurning.Add(burningObject);
        }
    }

    public void UnRegisterBurningObject(Burnable extinguishedObject)
    {
        if (objectsBurning.Contains(extinguishedObject))
        {
            objectsBurning.Remove(extinguishedObject);
        }
    }

    private void Update()
    {
        if (!SystemEnabled)
            return;

       
    }

    private static void SpreadFire(Vector3 pos, float spreadRadius)
    {
        List<Burnable> flammableObjects = GetFlammableObjectsInRange(pos, spreadRadius);
        foreach (var flammable in flammableObjects)
        {
            flammable.IgniteWithoutSpread();
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
                // only add to list if not already on fire
                if (!flammableComponent.IsOnFire)
                {
                    if (!flammable.Contains(flammableComponent))
                    {
                        flammable.Add(flammableComponent);
                    }
                }
            }
        }

        return flammable;
    }

}
