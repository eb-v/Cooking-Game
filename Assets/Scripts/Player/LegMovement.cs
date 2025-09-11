using UnityEngine;

public class LegMovement : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint _leftUpperLeg;
    [SerializeField] private ConfigurableJoint _rightUpperLeg;
    
    [Header("Right Leg X Y Z")]
    public float xL;
    public float yL;
    public float zL;
    [Header("Left Leg X Y Z")]
    public float xR;
    public float yR;
    public float zR;

    private void Update()
    {
        MoveLeg(_leftUpperLeg, xL, yL, zL);
        if (ManageJoints.IsJointAtTargetRotation(_leftUpperLeg, Quaternion.Euler(zL, xL, yL), 5f) && zL == -180f)
        {
            zL = -90f;
        }
        else if (ManageJoints.IsJointAtTargetRotation(_leftUpperLeg, Quaternion.Euler(zL, xL, yL), 5f) && zL == -90f)
        {
            zL = -180f;
        }

        Debug.Log(ManageJoints.IsJointAtTargetRotation(_leftUpperLeg, Quaternion.Euler(zL, xL, yL), 5f));
    }

    private void MoveLeg(ConfigurableJoint leg, float x, float y, float z)
    {
        ManageJoints.SetTargetRotation(leg, Quaternion.Euler(z, x, y));
    }



}
