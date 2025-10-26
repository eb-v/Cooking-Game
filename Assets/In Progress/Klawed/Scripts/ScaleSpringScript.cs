using UnityEngine;

public class ScaleSpringScript : MonoBehaviour, ISpringUI
{
    public float multiplier = 1f;
    public float baseScale = 1f;
    // this should match the name of the gameObject that has the SpringAPI component
    [SerializeField] private string _assignedChannel;

    void Start()
    {
        GenericEvent<SpringUpdateEvent>.GetEvent(_assignedChannel).AddListener(OnSpringValueRecieved);
    }


    public void OnSpringValueRecieved(float springValue)
    {
        float finalSpringValue = Mathf.Abs(springValue);
        Vector3 newScale = Vector3.one * (baseScale + finalSpringValue * multiplier);
        transform.localScale = newScale;
    }



}
