using UnityEngine;

public class PositionSpringScript : MonoBehaviour, ISpringUI
{
    public float yMultiplier; 
    public float xMultiplier;
    public float zMultiplier;
    // this should match the name of the gameObject that has the SpringAPI component
    [SerializeField] private string _assignedChannel;
    private string eventChannelName;

    public float nudgeStrength = 1f;

    [Header("Event Channel Name Options")]
    [SerializeField] private bool useGameObjectNameAsEventChannel = true;
    [SerializeField] private bool useCustomEventChannelName = false;
    [SerializeField] private bool useGameObjectIDAsEventChannel = false;

    private Vector3 initialPosition;


    private void Awake()
    {
        switch (true)
        {
            case true when useGameObjectNameAsEventChannel:
                eventChannelName = gameObject.name;
                break;
            case true when useCustomEventChannelName:
                // assignedEventChannel is already set to custom name
                eventChannelName = _assignedChannel;
                break;
            case true when useGameObjectIDAsEventChannel:
                eventChannelName = gameObject.GetInstanceID().ToString();
                break;
            default:
                eventChannelName = _assignedChannel;
                break;
        }
    }


    private void Start()
    {
        initialPosition = transform.localPosition;
        GenericEvent<SpringUpdateEvent>.GetEvent(eventChannelName).AddListener(OnSpringValueRecieved);
    }



    public void OnSpringValueRecieved(float springValue)
    {
        transform.localPosition = new Vector3(
            initialPosition.x + springValue * xMultiplier,
            initialPosition.y + springValue * yMultiplier,
            initialPosition.z + springValue * zMultiplier
        );
    }
}
