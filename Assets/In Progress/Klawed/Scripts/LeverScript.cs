using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField] private Transform leverPivotPoint;
    [SerializeField] private string _assignedChannel;
    [SerializeField] private int leverState = 0;
    [SerializeField] private float maxRotationAngle = 45f;
    [SerializeField] private float minRotationAngle = -45f;

    [SerializeField] private float snapForce = 10f;
    [SerializeField] private float currentAngle;
    [SerializeField] private float targetAngle;
    [SerializeField] private float newRotation = 0f;


    private void Start()
    {
        GenericEvent<LeverStateChangedEvent>.GetEvent(_assignedChannel).Invoke(leverState);
    }

    private void Update()
    {
        UpdateAngleValues();
        if (currentAngle != targetAngle)
        {
            ApplySnapForce();
        }
    }

    
    private void UpdateAngleValues()
    {
        // update target angle based on lever state
        targetAngle = 0f;
        if (leverState == -1)
        {
            targetAngle = minRotationAngle;
        }
        else if (leverState == 0)
        {
            targetAngle = 0;
        }
        else if (leverState == 1)
        {
            targetAngle = maxRotationAngle;
        }

        // update current angle based on lever rotation
        Quaternion quaternion = leverPivotPoint.localRotation;
        Vector3 eulerAngle = quaternion.eulerAngles;
        currentAngle = eulerAngle.z;
    }

    

    private void ApplySnapForce()
    {
        if (currentAngle < targetAngle)
        {
            newRotation = Mathf.MoveTowards(currentAngle, targetAngle, snapForce * Time.deltaTime);
        }
        else if (currentAngle > targetAngle)
        {
            newRotation = Mathf.MoveTowards(currentAngle, targetAngle, -snapForce * Time.deltaTime);
        }

        newRotation = Mathf.Clamp(newRotation, minRotationAngle, maxRotationAngle);

        leverPivotPoint.localRotation = Quaternion.Euler(0f, 0f, newRotation);
    }





}
