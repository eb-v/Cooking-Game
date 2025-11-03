using UnityEngine;

[RequireComponent(typeof(GrabDetection))]
public class GrabVisualScript : MonoBehaviour
{
    private GrabDetection grabDetection;
    public Transform handTransform;
    public Transform targetTransform;
    public LineRenderer lineRenderer;
    public float grabSpeed = 5f;


    private float currentLength = 0f;

    private void Awake()
    {
        grabDetection = GetComponent<GrabDetection>();

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
        if (grabDetection.grabbedObj != null)
        {
            targetTransform = grabDetection.grabbedObj.transform;
        }
        else
        {
            targetTransform = null;
        }

        if (grabDetection.isGrabbing && targetTransform != null)
        {
            lineRenderer.enabled = true;

            Vector3 start = handTransform.position;
            Vector3 end = targetTransform.position;

            // Animate extension
            currentLength = Mathf.MoveTowards(currentLength, 1f, grabSpeed * Time.deltaTime);
            Vector3 currentEnd = Vector3.Lerp(start, end, currentLength);

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, currentEnd);

            // Once fully extended
            if (currentLength >= 1f)
            {
                // Optionally: attach the object or show grab complete
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }


}




