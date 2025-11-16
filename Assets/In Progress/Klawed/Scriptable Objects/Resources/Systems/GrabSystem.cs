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
    public static void GrabObject(GameObject player, GameObject objToGrab)
    {
        if (!SystemEnabled)
            return;

        RagdollController rc = player.GetComponent<RagdollController>();

        rc.ExtendArmsOutward();

    }



    // detach object from hand
    public static void ReleaseObject(GameObject player, GameObject objToRelease)
    {
        if (!SystemEnabled)
            return;

        RagdollController rc = player.GetComponent<RagdollController>();
        rc.LowerArms();
    }
}