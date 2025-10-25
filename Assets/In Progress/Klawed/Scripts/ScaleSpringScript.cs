using UnityEngine;

[RequireComponent(typeof(SpringAPI))]
public class ScaleSpringScript : MonoBehaviour, ISpringUI
{
    public float multiplier = 1f;
    public float baseScale = 1f;

    void Start()
    {
        GenericEvent<SpringUpdateEvent>.GetEvent(gameObject.name).AddListener(OnSpringValueRecieved);
    }


    public void OnSpringValueRecieved(float springValue)
    {
        float finalSpringValue = Mathf.Abs(springValue);
        Vector3 newScale = Vector3.one * (baseScale + finalSpringValue * multiplier);
        transform.localScale = newScale;
    }



}
