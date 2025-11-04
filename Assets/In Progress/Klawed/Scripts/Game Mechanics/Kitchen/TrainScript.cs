using UnityEngine;
using System.Collections;

public class TrainScript : MonoBehaviour
{
    [SerializeField] private float delayBeforeLaunch = 2.0f;
    [SerializeField] private float trainSpeed = 100.0f;
    [SerializeField] private float stopPositionX = 0.0f;
    [SerializeField] private TrafficLightController trafficLight; // Reference to traffic light
    
    private Rigidbody trainRb;
    private bool hasLaunched = false;
    
    private void Start()
    {
        trainRb = GetComponent<Rigidbody>();
        GenericEvent<StartTrainEvent>.GetEvent(gameObject.name).AddListener(OnStartTrain);
    }
    
    private void Update()
    {
        if (hasLaunched)
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
            }
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
}