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
    
    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration  = 0.35f;
    [SerializeField] private float shakeMagnitude = 0.9f;
    
    public void Initialize(TrainData trainData)
    {
        this.trainData = trainData;
        trainRb = GetComponent<Rigidbody>();
        
        if (trainRb == null)
        {
            trainRb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Make train kinematic so it's not affected by physics
        trainRb.isKinematic = true;
        
        // Setup collider as trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }
    
    public void RunTrainLoop()
    {
        StartCoroutine(RandomTrainSpawnLoop());
    }
    
    private void FixedUpdate()
    {
        // Move train continuously when launched
        if (hasLaunched && !isResetting)
        {
            transform.position += Vector3.left * trainData.trainSpeed * Time.fixedDeltaTime;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Apply knockback without stopping the train
        Rigidbody hitRb = other.GetComponent<Rigidbody>();
        
        // Check if it's not part of the train itself
        if (hitRb != null && hitRb != trainRb && !other.transform.IsChildOf(transform))
        {
            // Calculate knockback direction (away from train)
            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
            
            // Add upward component for more dramatic effect
            knockbackDirection.y = trainData.upwardForce;
            knockbackDirection.Normalize();
            
            // Apply massive force
            hitRb.AddForce(knockbackDirection * trainData.knockbackForce, ForceMode.Impulse);
            
            // Optional: Add random spin for ragdoll effect
            hitRb.AddTorque(Random.insideUnitSphere * 300f);
            
            //Debug.Log("Train hit: " + other.gameObject.name);
        }
    }
    
    // Backup collision handler (in case trigger is disabled)
    private void OnCollisionEnter(Collision collision)
    {
        // Only use this if not using trigger
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger) return;
        
        Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();
        
        if (hitRb != null && hitRb != trainRb && !collision.transform.IsChildOf(transform))
        {
            Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
            knockbackDirection.y = trainData.upwardForce;
            knockbackDirection.Normalize();
            
            hitRb.AddForce(knockbackDirection * trainData.knockbackForce, ForceMode.Impulse);
            hitRb.AddTorque(Random.insideUnitSphere * 300f);
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
        hasLaunched = true;

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake();

        AudioManager.Instance?.PlaySFX("Train");
    }
    
    public void StopTrain()
    {
        hasLaunched = false;
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
        hasLaunched = false;
        
        yield return new WaitForSeconds(trainData.resetDelay);
        
        // Reset position and rotation
        trainRb.linearVelocity = Vector3.zero;
        trainRb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
        isResetting = false;
        
        // Turn light back to green
        if (trafficLight != null)
        {
            trafficLight.SetTrainActive(false);
        }
    }
}