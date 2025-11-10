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

    [Header("References")]
    [SerializeField] private Transform forwardTransform;
    [SerializeField] private GameObject crate;

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
    }


    private Vector3 UpdateMovementVector(Vector3 currentPos)
    {
        Vector3 targetPos = flightPath[currentPathIndex].position;
        Vector3 moveDir = (targetPos - currentPos).normalized;
        moveDir.x *= droneData.HorizontalSpeed;
        moveDir.y *= droneData.VerticalSpeed;
        moveDir.z *= droneData.HorizontalSpeed;

        return moveDir; 
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
        Vector3 moveVector = UpdateMovementVector(transform.position);
        rb.MovePosition(rb.position + moveVector * Time.deltaTime);

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
        FixedJoint joint = crate.GetComponent<FixedJoint>();
        Destroy(joint);
        currentState = DroneState.Idle;
    }

    

    private void RunEndPathReachedLogic()
    {
        ResetValues();
        GenericEvent<DroneCompletedPath>.GetEvent("DroneManager").Invoke(this.gameObject);
    }

    private void ResetValues()
    {
        currentPathIndex = 0;
        currentState = DroneState.Idle;
    }

    public void SetCrateReference(GameObject crate)
    {
        this.crate = crate;
    }

}
