using UnityEngine;

public class ScaleSpringScript : MonoBehaviour, ISpringUI
{
    public float multiplier = 1f;
    public float baseScale = 1f;
    // this should match the name of the gameObject that has the SpringAPI component
    [SerializeField] private string _assignedChannel;
    [SerializeField] private bool useAssignedChannel = true;
    [SerializeField] private bool useGameObjectNameAsChannel = false;
    [SerializeField] private bool useGameObjectIDsChannel = false;

    void Start()
    {
        if (useAssignedChannel)
        {
            GenericEvent<SpringUpdateEvent>.GetEvent(_assignedChannel).AddListener(OnSpringValueRecieved);
        }
        else if (useGameObjectNameAsChannel)
        {
            GenericEvent<SpringUpdateEvent>.GetEvent(gameObject.name).AddListener(OnSpringValueRecieved);
        }
        else if (useGameObjectIDsChannel)
        {
            GenericEvent<SpringUpdateEvent>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(OnSpringValueRecieved);
        }
        else
        {
            GenericEvent<SpringUpdateEvent>.GetEvent(_assignedChannel).AddListener(OnSpringValueRecieved);
        }
    }


    public void OnSpringValueRecieved(float springValue)
    {
        float finalSpringValue = Mathf.Abs(springValue);
        Vector3 newScale = Vector3.one * (baseScale + finalSpringValue * multiplier);
        transform.localScale = newScale;
    }



}
