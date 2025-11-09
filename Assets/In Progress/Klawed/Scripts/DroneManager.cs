using UnityEngine;

public class DroneManager : MonoBehaviour
{

    [SerializeField] private Transform droneSpawnPoint;
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private GameObject cratePrefab;
    private Transform crateSnapPoint;

    void Start()
    {
        GameObject drone = SpawnDrone();
        GameObject crate = SpawnCrate();
        ConnectCrateToDrone(drone, crate);
    }

    private GameObject SpawnDrone()
    {
        GameObject drone = ObjectPoolManager.SpawnObject(dronePrefab, droneSpawnPoint.position, dronePrefab.transform.rotation);
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
