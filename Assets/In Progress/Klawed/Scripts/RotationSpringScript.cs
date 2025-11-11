using UnityEngine;

public class RotationSpringScript : MonoBehaviour, ISpringUI
{
    private string eventChannelName;
    [SerializeField] private string customEventChannelName;
    [SerializeField] private Vector3 rotationMultiplier;
    private Vector3 initialRotation;
    [Header("Event Channel Name Options")]
    [SerializeField] private bool useGameObjectNameAsEventChannel = true;
    [SerializeField] private bool useCustomEventChannelName = false;
    [SerializeField] private bool useGameObjectIDAsEventChannel = false;

    [Header("Rotation Settings")]
    [SerializeField] private bool setAdditively = false;


    private void Awake()
    {
        switch (true)
        {
            case true when useGameObjectNameAsEventChannel:
                eventChannelName = gameObject.name;
                break;
            case true when useCustomEventChannelName:
                // assignedEventChannel is already set to custom name
                eventChannelName = customEventChannelName;
                break;
            case true when useGameObjectIDAsEventChannel:
                eventChannelName = gameObject.GetInstanceID().ToString();
                break;
            default:
                eventChannelName = customEventChannelName;
                break;
        }
    }

    private void Start()
    {
        initialRotation = transform.localEulerAngles;
        GenericEvent<SpringUpdateEvent>.GetEvent(eventChannelName).AddListener(OnSpringValueRecieved);
    }

    public void OnSpringValueRecieved(float springValue)
    {
        
        Vector3 newRotation = new Vector3(
            rotationMultiplier.x * springValue,
            rotationMultiplier.y * springValue,
            rotationMultiplier.z * springValue
        );

        if (!setAdditively)
        {
            transform.localEulerAngles = newRotation;
        }
        else
        {
            transform.localEulerAngles += newRotation;
        }
    }

    
}
