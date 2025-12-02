using UnityEngine;

[RequireComponent(typeof(GrabScript))]
public class GrabVisualScript : MonoBehaviour
{
    private GrabScript grabDetection;
    public Transform handTransform;
    public Transform targetTransform;
    public LineRenderer lineRenderer;
    public float grabSpeed = 5f;


    private float currentLength = 0f;

    private void Awake()
    {
        grabDetection = GetComponent<GrabScript>();

       // GenericEvent<OnHandCollisionEnter>.GetEvent(transform.root.name).AddListener(OnObjectGrabbed);
        //GenericEvent<ObjectReleased>.GetEvent(gameObject.name).AddListener(ObjectReleased);
    }

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    private void OnObjectGrabbed(GameObject inputGameObject, GameObject grabbedObject)
    {
        if (targetTransform != null)
            return;
        targetTransform = grabbedObject.transform;
    }

    private void ObjectReleased()
    {
        targetTransform = null;
    }

    private void Update()
    {
        
    }


}




