using System.Runtime.CompilerServices;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    private TrainScript trainScript;
    [SerializeField] private TrainData trainData;
    [SerializeField] private TrafficLightController trafficLight; // Reference to traffic light

    private bool hasLaunched = false;
    private bool isResetting = false;

    private static TrainManager instance;

    public static TrainManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<TrainManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("Train Manager");
                    instance = singletonObject.AddComponent<TrainManager>();
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        trainScript = FindFirstObjectByType<TrainScript>();
        if (trainScript == null)
        {
            Debug.LogError("TrainScript not found in the scene!");
        }
        trainScript.Initialize(trainData);
       // StartTrainLoop();
    }

    private void StartTrainLoop()
    {
        trainScript.RunTrainLoop();
    }

    public void LaunchTrain()
    {
        trainScript.LaunchTrain();
    }

    public void RunUpdateLogic()
    {
        if (hasLaunched && !isResetting)
        {
            if (transform.position.x <= trainData.stopPositionX)
            {
                trainScript.StopTrain();
                hasLaunched = false;

                // Start reset countdown
                trainScript.ResetTrain();
            }
        }
    }

}
