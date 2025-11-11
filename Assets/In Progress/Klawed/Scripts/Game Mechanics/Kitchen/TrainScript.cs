using UnityEngine;
using System.Collections;

public class TrainScript : MonoBehaviour
{
    [SerializeField] private float delayBeforeLaunch = 2.0f;
    [SerializeField] private float trainSpeed = 100.0f;
    [SerializeField] private float stopPositionX = 0.0f;
    [SerializeField] private float resetDelay = 10.0f; // Time before train resets after stopping
    [SerializeField] private float randomCallInterval = 20.0f; // Random call every 20 seconds
    [SerializeField] private TrafficLightController trafficLight; // Reference to traffic light
    [SerializeField] private float knockbackForce = 1000f; // Force applied when hitting objects
    [SerializeField] private float upwardForce = 0.5f; // Upward component for knockback (0-1)
    
    private Rigidbody trainRb;
    private bool hasLaunched = false;
    private bool isResetting = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    private void Start()
    {
        trainRb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // Start the random train spawning loop
        StartCoroutine(RandomTrainSpawnLoop());
        
        // OLD CODE - Manual button trigger
        //GenericEvent<StartTrainEvent>.GetEvent(gameObject.name).AddListener(OnStartTrain);
    }
    
    private void Update()
    {
        if (hasLaunched && !isResetting)
        {
            if (transform.position.x <= stopPositionX)
            {
                StopTrain();
                hasLaunched = false;
                
                // Turn light back to green when train stops
                if (trafficLight != null)
                {
                    trafficLight.SetTrainActive(false);
                }
                
                // Start reset countdown
                StartCoroutine(ResetTrainAfterDelay());
            }
        }
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
        knockbackDirection.y = upwardForce;
        knockbackDirection.Normalize();
        
        // Apply massive force
        hitRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        
        // Optional: Add random spin for ragdoll effect
        hitRb.AddTorque(Random.insideUnitSphere * 300f);
        
        Debug.Log("Train hit: " + collision.gameObject.name);
    }
}
    
    // Random spawn loop - calls train randomly within each interval
    private IEnumerator RandomTrainSpawnLoop()
    {
        while (true)
        {
            // Wait a random time within the interval
            float randomTime = Random.Range(0f, randomCallInterval);
            yield return new WaitForSeconds(randomTime);
            
            // Only spawn if train isn't already running or resetting
            if (!hasLaunched && !isResetting)
            {
                OnStartTrain();
            }
            
            // Wait for the remaining time to complete the cycle
            yield return new WaitForSeconds(randomCallInterval - randomTime);
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
        
        yield return new WaitForSeconds(delayBeforeLaunch);
        LaunchTrain();
    }
    
    private void LaunchTrain()
    {
        trainRb.AddForce(Vector3.left * trainSpeed, ForceMode.VelocityChange);
        hasLaunched = true;
    }
    
    private void StopTrain()
    {
        trainRb.linearVelocity = Vector3.zero;
    }
    
    // Reset train position after delay
    private IEnumerator ResetTrainAfterDelay()
    {
        isResetting = true;
        yield return new WaitForSeconds(resetDelay);
        
        // Reset position and rotation
        trainRb.linearVelocity = Vector3.zero;
        trainRb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
        isResetting = false;
        Debug.Log("Train reset to starting position");
    }
}