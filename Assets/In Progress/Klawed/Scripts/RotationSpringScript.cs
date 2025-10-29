using UnityEngine;

public class RotationSpringScript : MonoBehaviour, ISpringUI
{
    [SerializeField] private string _assignedChannel;
    [SerializeField] private Vector3 rotationMultiplier;

    private void Start()
    {
        GenericEvent<SpringUpdateEvent>.GetEvent(_assignedChannel).AddListener(OnSpringValueRecieved);
    }

    public void OnSpringValueRecieved(float springValue)
    {
        Vector3 newRotation = new Vector3(
            rotationMultiplier.x * springValue,
            rotationMultiplier.y * springValue,
            rotationMultiplier.z * springValue
        );

        
        transform.localEulerAngles = newRotation;
    }

    
}
