using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Grab System", menuName = "Systems/Grab System")]
public class GrabSystem : ScriptableObject
{
    private static GrabSystem instance;


    public static GrabSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GrabSystem>("Systems/Grab System");
            }
            return instance;
        }
    }

    [SerializeField] private bool systemEnabled = true;

    public static bool SystemEnabled
    {
        get { return Instance.systemEnabled; }
        set { Instance.systemEnabled = value; }
    }

    private void OnEnable()
    {
        SystemEnabled = systemEnabled;
    }

    private void OnValidate()
    {
        SystemEnabled = systemEnabled;
    }


    // attach object to hand
    public static void GrabObject(GameObject player, GameObject physicsObj, GrabData grabData, ArmRotationData armRotationData)
    {
        if (!SystemEnabled)
            return;
        Rigidbody rb = physicsObj.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        RagdollController rc = player.GetComponent<RagdollController>();
        CoroutineRunner.Instance.StartCoroutine(GrabObjectCoroutine(rc, physicsObj, grabData, armRotationData));
        IGrabable grabable = physicsObj.GetComponent<IGrabable>();
        GrabScript gs = player.GetComponent<GrabScript>();
        gs.grabbedObject = grabable;
    }

    private static IEnumerator GrabObjectCoroutine(RagdollController rc, GameObject physicsObj, GrabData grabData, ArmRotationData armRotationData)
    {
        rc.ExtendArmsOutward(grabData);
        
        ConfigurableJoint upperRightArm = rc.RagdollDict["UpperRightArm"].Joint;
        ConfigurableJoint upperLeftArm = rc.RagdollDict["UpperLeftArm"].Joint;
        ConfigurableJoint lowerRightArm = rc.RagdollDict["LowerRightArm"].Joint;
        ConfigurableJoint lowerLeftArm = rc.RagdollDict["LowerLeftArm"].Joint;

        PoseHelper.SnapJointToRotation(upperRightArm, Quaternion.Euler(armRotationData.UpperRightArmRotation));
        PoseHelper.SnapJointToRotation(lowerRightArm, Quaternion.Euler(armRotationData.LowerRightArmRotation));
        PoseHelper.SnapJointToRotation(upperLeftArm, Quaternion.Euler(armRotationData.UpperLeftArmRotation));
        PoseHelper.SnapJointToRotation(lowerLeftArm, Quaternion.Euler(armRotationData.LowerLeftArmRotation));
        yield return null;
        Debug.LogError("Snapped Joints");
        //Rigidbody rb = physicsObj.GetComponent<Rigidbody>();
        //rb.linearVelocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;

        //GameObject lowerRightArm = rc.RagdollDict["LowerRightArm"].gameObject;
        //GameObject lowerLeftArm = rc.RagdollDict["LowerLeftArm"].gameObject;
        //GameObject body = rc.RagdollDict["Body"].gameObject;
        //physicsObj.GetComponent<Collider>().enabled = false;
        //physicsObj.transform.position = body.transform.TransformPoint(grabData.position);
        //physicsObj.transform.rotation = body.transform.rotation * grabData.rotation;
        //lowerRightArm.AddComponent<FixedJoint>().connectedBody = physicsObj.GetComponent<Rigidbody>();
        //lowerLeftArm.AddComponent<FixedJoint>().connectedBody = physicsObj.GetComponent<Rigidbody>();
        //yield return new WaitForSeconds(0.5f);
        //physicsObj.GetComponent<Collider>().enabled = true;
        yield return null;
    }



    // detach object from hand
    public static void ReleaseObject(GameObject player)
    {
        if (!SystemEnabled)
            return;

        GrabScript grabScript = player.GetComponent<GrabScript>();
        grabScript.grabbedObject = null;
        RagdollController rc = player.GetComponent<RagdollController>();
        GameObject lowerRightArm = rc.RagdollDict["LowerRightArm"].gameObject;
        GameObject lowerLeftArm = rc.RagdollDict["LowerLeftArm"].gameObject;
        Destroy(lowerRightArm.GetComponent<FixedJoint>());
        Destroy(lowerLeftArm.GetComponent<FixedJoint>());
        rc.LowerArms();
    }
}