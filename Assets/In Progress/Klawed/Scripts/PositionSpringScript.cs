using UnityEngine;

public class PositionSpringScript : MonoBehaviour, ISpringUI
{
    public float verticalMultiplier; 
    public float horizontalMultiplier;
    // this should match the name of the gameObject that has the SpringAPI component
    [SerializeField] private string _assignedChannel;

    private Vector3 initialPosition;



    

    private void Start()
    {
        initialPosition = transform.localPosition;
        GenericEvent<SpringUpdateEvent>.GetEvent(_assignedChannel).AddListener(OnSpringValueRecieved);
    }



    public void OnSpringValueRecieved(float springValue)
    {
        transform.localPosition = new Vector3(
            initialPosition.x + springValue * horizontalMultiplier,
            initialPosition.y + springValue * verticalMultiplier,
            initialPosition.z
        );
    }
}
