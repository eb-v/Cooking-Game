using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class DroneScript : MonoBehaviour
{
    private enum DroneState
    {
        Idle,
        MovingToDestination,
        DroppingCrate,
        EndPathReached
    }


    public Transform crateSnapPoint;

    private Drone droneData;
    private List<Transform> flightPath;
    private int dropNodeIndex;

    private Vector3 currentTargetDestination;
    private Rigidbody rb;

    [Header("Drone Debug Info")]
    [SerializeField] private Vector3 currentVelocity;
    [SerializeField] private int currentPathIndex = 0;
    [SerializeField] private DroneState currentState = DroneState.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Drone droneData, List<Transform> flightPath, int dropNodeIndex)
    {
        this.droneData = droneData;
        this.flightPath = flightPath;
        this.dropNodeIndex = dropNodeIndex;
    }

    private void Update()
    {
        RunStateLogic();

        UpdateRotation();
    }


    private Vector3 GetMovementVector(Vector3 currentPos)
    {
        Vector3 targetPos = flightPath[currentPathIndex].position;
        Vector3 direction = (targetPos - currentPos).normalized;
        Vector3 newVelocity = new Vector3(direction.x * droneData.HorizontalSpeed,
                                      direction.y * droneData.VerticalSpeed,
                                      direction.z * droneData.HorizontalSpeed);
        return newVelocity;
    }

    private void RunStateLogic()
    {
        switch (currentState)
        {
            case DroneState.Idle:
                RunIdleLogic();
                break;
            case DroneState.MovingToDestination:
                RunMovingToDestinationLogic();
                break;
            case DroneState.DroppingCrate:
                RunDroppingCrateLogic();
                break;
            case DroneState.EndPathReached:
                RunEndPathReachedLogic();
                break;
        }
    }

    // Idle state just updates the flight Destination to begin moving to next node
    private void RunIdleLogic()
    {
        currentTargetDestination = flightPath[currentPathIndex].position;
        currentState = DroneState.MovingToDestination;
    }

    private void RunMovingToDestinationLogic()
    {
        Vector3 newVel = GetMovementVector(transform.position);
        rb.MovePosition(rb.position + newVel * Time.deltaTime);


        float distanceToTarget = Vector3.Distance(transform.position, currentTargetDestination);
        if (distanceToTarget < 0.5f)
        {
            if (currentPathIndex >= flightPath.Count - 1)
            {
                currentState = DroneState.EndPathReached;
                return;
            }
            // reached target node, check if it's drop node
            if (currentPathIndex == dropNodeIndex)
            {
                currentState = DroneState.DroppingCrate;
            }
            else
            {
                currentState = DroneState.Idle;
            }
            currentPathIndex++;
        }

    }

    private void RunDroppingCrateLogic()
    {
        // Implement crate dropping logic here
        Debug.Log("Crate Dropped");
        currentState = DroneState.Idle;
    }

    private void UpdateRotation()
    {
        currentVelocity = rb.linearVelocity;
        if (currentVelocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void RunEndPathReachedLogic()
    {
        ObjectPoolManager.ReturnObjectToPool(this.gameObject);
    }

}
