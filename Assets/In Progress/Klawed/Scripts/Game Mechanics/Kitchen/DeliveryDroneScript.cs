using System.Collections;
using UnityEngine;

public class DeliveryDroneScript : MonoBehaviour
{
    private float startingXPos;
    [SerializeField] private float endingXPos;
    [SerializeField] private float delay;
    [SerializeField] private float dropCrateXPos;
    [SerializeField] private string _assignedChannel = "DefaultChannel";
    public bool deliveryMode = false;
    public float speed = 5f;
    public bool crateDropped = false;
    

    private void Start()
    {
        startingXPos = transform.position.x;
        GenericEvent<IngredientOrderedEvent>.GetEvent(_assignedChannel).AddListener(OnShipmentOrdered);
    }


    private void OnShipmentOrdered(GameObject ingredient)
    {
        StartCoroutine(StartDeliveryCoroutine());
    }

    private IEnumerator StartDeliveryCoroutine()
    {
        yield return new WaitForSeconds(delay);
        StartDelivery();

    }

    private void StartDelivery() => deliveryMode = true;

    private void EndDelivery() => deliveryMode = false;


    private void Update()
    {
        if (deliveryMode)
        {
            Vector3 currentPos = transform.position;

            if (currentPos.x <= dropCrateXPos && !crateDropped)
            {
                DropCrate();
            }


            if (currentPos.x > endingXPos)
            {
                currentPos.x -= speed * Time.deltaTime;
                transform.position = currentPos;
            }
            else
            {
                EndDelivery();
                ResetPosition();
            }
        }
    }

    private void ResetPosition()
    {
        Vector3 resetPos = transform.position;
        resetPos.x = startingXPos;
        transform.position = resetPos;
    }

    private void DropCrate()
    {
        GameObject crate = transform.Find("IngredientShipment").gameObject;
        if (crate != null)
        {
            crate.transform.parent = null;
            Rigidbody crateRb = crate.GetComponent<Rigidbody>();
            if (crateRb != null)
            {
                crateRb.isKinematic = false;
            }
        }
        crateDropped = true;
    }

}
