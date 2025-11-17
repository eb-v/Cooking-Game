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
    public static void GrabObject(GameObject player, GameObject physicsObj, GrabData grabData)
    {
        if (!SystemEnabled)
            return;
        RagdollController rc = player.GetComponent<RagdollController>();
        CoroutineRunner.Instance.StartCoroutine(GrabObjectCoroutine(rc, physicsObj, grabData));

    }

    private static IEnumerator GrabObjectCoroutine(RagdollController rc, GameObject physicsObj, GrabData grabData)
    {
        rc.ExtendArmsOutward(grabData);
        yield return new WaitForSeconds(0.1f);
        GameObject lowerRightArm = rc.RagdollDict["LowerRightArm"].gameObject;
        GameObject lowerLeftArm = rc.RagdollDict["LowerLeftArm"].gameObject;
        GameObject body = rc.RagdollDict["Body"].gameObject;
        physicsObj.GetComponent<Collider>().enabled = false;
        physicsObj.transform.position = body.transform.TransformPoint(grabData.position);
        physicsObj.transform.rotation = body.transform.rotation * grabData.rotation;
        lowerRightArm.AddComponent<FixedJoint>().connectedBody = physicsObj.GetComponent<Rigidbody>();
        lowerLeftArm.AddComponent<FixedJoint>().connectedBody = physicsObj.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(0.5f);
        physicsObj.GetComponent<Collider>().enabled = true;

    }



    // detach object from hand
    public static void ReleaseObject(GameObject player)
    {
        if (!SystemEnabled)
            return;

        RagdollController rc = player.GetComponent<RagdollController>();
        GameObject lowerRightArm = rc.RagdollDict["LowerRightArm"].gameObject;
        GameObject lowerLeftArm = rc.RagdollDict["LowerLeftArm"].gameObject;
        Destroy(lowerRightArm.GetComponent<FixedJoint>());
        Destroy(lowerLeftArm.GetComponent<FixedJoint>());
        rc.LowerArms();
    }
}