using UnityEngine;
using System.Collections;

public class TrainScript : MonoBehaviour
{
    [SerializeField] private TrafficLightController trafficLight; // Reference to traffic light
    private TrainData trainData;

    private Rigidbody trainRb;
    private bool hasLaunched = false;
    private bool isResetting = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;


    public void Initialize(TrainData trainData)
    {
        trainRb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Start the random train spawning loop
        this.trainData = trainData;
    }



    public void RunTrainLoop()
    {
        StartCoroutine(RandomTrainSpawnLoop());
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Check if we hit something with a Rigidbody (like the player)
        Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();

        // Check if it's not part of the train itself
        if (hitRb != null && hitRb != trainRb && !collision.transform.IsChildOf(transform))
        {
            // Calculate knockback direction (away from train)
            Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;

            // Add upward component for more dramatic effect
            knockbackDirection.y = trainData.upwardForce;
            knockbackDirection.Normalize();

            // Apply massive force
            hitRb.AddForce(knockbackDirection * trainData.knockbackForce, ForceMode.Impulse);

            // Optional: Add random spin for ragdoll effect
            hitRb.AddTorque(Random.insideUnitSphere * 300f);

            //Debug.Log("Train hit: " + collision.gameObject.name);
        }
    }

    // Random spawn loop - calls train randomly within each interval
    private IEnumerator RandomTrainSpawnLoop()
    {
        while (true)
        {
            // Wait a random time within the interval
            float randomTime = Random.Range(0f, trainData.randomCallInterval);
            yield return new WaitForSeconds(randomTime);

            // Only spawn if train isn't already running or resetting
            if (!hasLaunched && !isResetting)
            {
                OnStartTrain();
            }

            // Wait for the remaining time to complete the cycle
            yield return new WaitForSeconds(trainData.randomCallInterval - randomTime);
        }
    }

    // OLD CODE - Manual trigger method
    //public void OnStartTrain()
    //{
    //    StartCoroutine(TrainMovementCoroutine());
    //}

    public void OnStartTrain()
    {
        StartCoroutine(TrainMovementCoroutine());
    }

    // Launch train after delay
    private IEnumerator TrainMovementCoroutine()
    {
        // Turn light red when train is about to move
        if (trafficLight != null)
        {
            trafficLight.SetTrainActive(true);
        }

        yield return new WaitForSeconds(trainData.delayBeforeLaunch);
        LaunchTrain();
    }

    private void LaunchTrain()
    {
        trainRb.AddForce(Vector3.left * trainData.trainSpeed, ForceMode.VelocityChange);
        hasLaunched = true;
    }

    public void StopTrain()
    {
        trainRb.linearVelocity = Vector3.zero;
    }

    public void ResetTrain()
    {
        StartCoroutine(ResetTrainAfterDelayCoroutine());
    }

    // Reset train position after delay
    private IEnumerator ResetTrainAfterDelayCoroutine()
    {
        isResetting = true;
        yield return new WaitForSeconds(trainData.resetDelay);

        // Reset position and rotation
        trainRb.linearVelocity = Vector3.zero;
        trainRb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        isResetting = false;
    }
}