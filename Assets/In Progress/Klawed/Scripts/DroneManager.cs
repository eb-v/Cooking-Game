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

    private Transform crateSnapPoint;
    

    private void Awake()
    {
        InitializeFlightPath();
    }

    void Start()
    {
        GameObject drone = SpawnDrone();
        //GameObject crate = SpawnCrate();
        //ConnectCrateToDrone(drone, crate);
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

        return drone;
    }

    private GameObject SpawnCrate()
    {
        GameObject crate = ObjectPoolManager.SpawnObject(cratePrefab, crateSnapPoint.position, Quaternion.identity);
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
    }

    public void ReleaseCreate()
    {
        GenericEvent<ReleaseCrate>.GetEvent("releaseCrate").Invoke();
    }


}
