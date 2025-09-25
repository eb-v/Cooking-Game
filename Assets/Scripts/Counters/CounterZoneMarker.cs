using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CounterZoneMarker : MonoBehaviour
{
    public GameObject markerPrefab; 
    private GameObject markerInstance;
    private BoxCollider boxCollider;
    [SerializeField] private GameObject counterParent; //assign same parent

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (!boxCollider.isTrigger)
            boxCollider.isTrigger = true;

        if (markerPrefab != null)
        {
            markerInstance = Instantiate(markerPrefab, transform);
            markerInstance.transform.localPosition = boxCollider.center;
            markerInstance.transform.localRotation = Quaternion.identity;
            markerInstance.transform.localScale = boxCollider.size;
            markerInstance.SetActive(false);
        }

        if (counterParent == null) {
            counterParent = transform.parent.gameObject;
        }

        GenericEvent<EnterCounter>.GetEvent(counterParent.GetInstanceID()).AddListener(Show);
        GenericEvent<ExitCounter>.GetEvent(counterParent.GetInstanceID()).AddListener(Hide);

        Debug.Log("Subscribed to EnterCounter and ExitCounter for ID " + gameObject.GetInstanceID());

    }

    public void Show()
    {
        if (markerInstance != null){
            markerInstance.SetActive(true);
        }
        Debug.Log("Marker shown");
    }

    public void Hide()
    {
        if (markerInstance != null) {
            markerInstance.SetActive(false);
        }
        Debug.Log("Marker hidden");
    }
}
