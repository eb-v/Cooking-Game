using System.Collections.Generic;
using UnityEngine;

public class testRotationSnapping : MonoBehaviour
{
    [SerializeField] private RagdollController rc;
    [SerializeField] private PoseData poseData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Update()
    {
        //foreach (KeyValuePair<string, RagdollJoint> kvp in rc.RagdollDict)
        //{
        //    ConfigurableJoint joint = kvp.Value.Joint;
        //    joint.targetRotation = poseData.poseRotations[kvp.Key];
        //}

    }

    public void SetRotations()
    {
        foreach (KeyValuePair<string, RagdollJoint> kvp in rc.RagdollDict)
        {
            if (kvp.Key == "Root")
                continue;

            ConfigurableJoint joint = kvp.Value.Joint;


            joint.transform.rotation = poseData.poseWorldRotations[kvp.Key];
            joint.targetRotation = poseData.poseJointTargetRotations[kvp.Key];
        }
    }


}
