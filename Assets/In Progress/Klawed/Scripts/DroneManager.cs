using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DroneManager : MonoBehaviour
{
    [Header("References")]
    //[SerializeField] private Transform droneSpawnPoint;
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private GameObject cratePrefab;
    [SerializeField] private Drone droneSO;


    [Header("Drone Pathing")]
    [SerializeField] private List<Transform> flightPath;
    [SerializeField] private GameObject flightPathContainer;
    private int dropNodeIndex;

    [Header("Debug Info")]
    [SerializeField] private bool isDroneActive = false;
    //[SerializeField] private int ingredientsToSpawn = 4;

    private Transform crateSnapPoint;

    

    private void Awake()
    {
        InitializeFlightPath();

        GenericEvent<DroneCompletedPath>.GetEvent("DroneManager").AddListener(DespawnDrone);
        GenericEvent<DroneDeliveryCalled>.GetEvent("DroneManager").AddListener(CallDroneDelivery);
    }

    

    private void InitializeFlightPath()
    {
        foreach (Transform node in flightPathContainer.transform)
        {
            flightPath.Add(node);
            if (node.name == "DropNode")
            {
                dropNodeIndex = flightPath.IndexOf(node);
            }
        }
    }


    private GameObject SpawnDrone()
    {
        GameObject drone = ObjectPoolManager.SpawnObject(dronePrefab, flightPath[0].position, dronePrefab.transform.rotation);
        DroneScript droneScript = drone.GetComponent<DroneScript>();
        droneScript.Initialize(droneSO, flightPath, dropNodeIndex);

        crateSnapPoint = drone.GetComponent<DroneScript>().crateSnapPoint;
        isDroneActive = true;

        return drone;
    }

    private GameObject SpawnCrate()
    {
        //GameObject crate = ObjectPoolManager.SpawnObject(cratePrefab, crateSnapPoint.position, Quaternion.identity);
        GameObject crate = Instantiate(cratePrefab, crateSnapPoint.position, Quaternion.identity);
        crate.transform.position = crateSnapPoint.position;
        return crate;
    }

    private void ConnectCrateToDrone(GameObject drone, GameObject crate)
    {
        FixedJoint joint = crate.GetComponent<FixedJoint>();
        if (joint == null)
        {
            joint = crate.AddComponent<FixedJoint>();
        }

        joint.connectedBody = drone.GetComponent<Rigidbody>();
        drone.GetComponent<DroneScript>().SetCrateReference(crate);
    }

    private void CallDroneDelivery(Ingredient orderedIngredient)
    {
        if (!isDroneActive)
        {
            GameObject drone = SpawnDrone();
            GameObject crate = SpawnCrate();
            ConnectCrateToDrone(drone, crate);
            CrateScript crateScript = crate.GetComponent<CrateScript>();
            crateScript.SetIngredient(orderedIngredient.Prefab);
        }
        else
        {
            Debug.Log("Delivery is in Progress");
        }
    }

    private void DespawnDrone(GameObject drone)
    {
        ObjectPoolManager.ReturnObjectToPool(drone);
        isDroneActive = false;
    }

    



}
