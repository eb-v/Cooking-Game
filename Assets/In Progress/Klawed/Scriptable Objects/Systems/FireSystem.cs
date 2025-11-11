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
                instance = Resources.Load<FireSystem>("Fire System");
            }
            return instance;
        }
    }

    [SerializeField] private bool systemEnabled = true;


    [Header("References")]
    [SerializeField] private GameObject firePrefab;

    // (GameObject) -> (fire Controller)
    private static Dictionary<GameObject, FireController> ignitedObjects = new Dictionary<GameObject, FireController>();

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


    public static void IgniteObject(GameObject objToIgnite)
    {
        if (!SystemEnabled)
            return;
        
        GameObject fireObj = Instantiate(Instance.firePrefab, objToIgnite.transform.position, Quaternion.identity, objToIgnite.transform);
        FireController fireController = fireObj.GetComponent<FireController>();
        ignitedObjects[objToIgnite] = fireController;
    }

    public static void ExtinguishObject(GameObject objToExtinguish)
    {
        if (!SystemEnabled)
            return;
        if (ignitedObjects.ContainsKey(objToExtinguish))
        {
            FireController fireController = ignitedObjects[objToExtinguish];
            ignitedObjects.Remove(objToExtinguish); 
            fireController.StopFire();
        }
        else
        {
            Debug.LogWarning($"[FireSystem] Attempted to extinguish object that is not on fire: {objToExtinguish.name}");
        }
    }

}
