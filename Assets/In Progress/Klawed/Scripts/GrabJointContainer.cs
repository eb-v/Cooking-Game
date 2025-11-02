using UnityEngine;
using System.Collections.Generic;

public class GrabJointContainer : MonoBehaviour
{
    [SerializeField] private UDictionary<int, ConfigurableJoint> grabJointMap = new UDictionary<int, ConfigurableJoint>();


    public ConfigurableJoint GetGrabJoint(int handID)
    {
        if (grabJointMap.ContainsKey(handID))
        {
            return grabJointMap[handID];
        }
        else
        {
            return null;
        }
    }

    public void AddGrabJoint(int handID, ConfigurableJoint grabJoint)
    {
        grabJointMap.Add(handID, grabJoint);
    }

    public void RemoveGrabJoint(int handID)
    {
        if (grabJointMap.ContainsKey(handID))
        {
            grabJointMap.Remove(handID);
        }
    }


}
