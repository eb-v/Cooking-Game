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

    [SerializeField] private bool systemEnabled = true;

    private HashSet<Burnable> spreadThisFrame = new HashSet<Burnable>();
    private List<Burnable> objectsBurning = new List<Burnable>();

    // this is used to target a single object to spread fire to for each burning object
    private Dictionary<Burnable, Burnable> objectToSpreadFireTo = new Dictionary<Burnable, Burnable>();
    private HashSet<Burnable> removeFromObjectToSpreadFireTo = new HashSet<Burnable>();
    private bool fireLimitReached = false;



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

    public void SpreadFire(Vector3 position, float spreadRadius, Burnable fireOrigin)
    {
        if (!SystemEnabled)
            return;

        if (fireLimitReached)
            return;

        List<Burnable> burnablesInRange = GetFlammableObjectsInRange(position, spreadRadius);
        // no burnable objects in range, exit function
        if (burnablesInRange.Count == 0)
            return;

        // does fire origin already have a target to spread fire to?
        if (!objectToSpreadFireTo.ContainsKey(fireOrigin))
        {
            // choose a single random burnable in range to spread fire to
            int burnablesCount = burnablesInRange.Count;
            int randomIndex = Random.Range(0, burnablesCount);
            objectToSpreadFireTo.Add(fireOrigin, burnablesInRange[randomIndex]);
        }
        else
        {
            // fire origin already has a target to spread fire to
            // spread fire to that target
            Burnable targetBurnable = objectToSpreadFireTo[fireOrigin];

            // has this target burnAmount already been modified this frame?
            if (!spreadThisFrame.Contains(targetBurnable))
            {
                float value = Random.Range(0f, settings.burnMultiplier);
                targetBurnable.ModifyBurnProgress(value * Time.deltaTime);
                spreadThisFrame.Add(targetBurnable);
            }
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

        if (objectsBurning.Count > settings.maxSimultaneousFires)
        {
            fireLimitReached = true;
        }
        else
        {
            fireLimitReached = false;
        }

        foreach (KeyValuePair<Burnable, Burnable> kvp in objectToSpreadFireTo)
        {
            if (kvp.Value.IsOnFire)
            {
                removeFromObjectToSpreadFireTo.Add(kvp.Key);
            }
        }

        foreach (Burnable obj in removeFromObjectToSpreadFireTo)
        {
            objectToSpreadFireTo.Remove(obj);
        }

        removeFromObjectToSpreadFireTo.Clear();
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

    private void LateUpdate()
    {
        spreadThisFrame.Clear();
    }
}
