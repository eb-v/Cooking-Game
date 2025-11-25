using UnityEngine;

public class FollowPhysicsBody : MonoBehaviour
{
    public Transform physicsTransform;
    public bool followPosition = true;
    public bool followRotation = true;

    private void LateUpdate()
    {
        if (physicsTransform != null)
        {
            if (followPosition)
                transform.position = physicsTransform.position;
            if (followRotation)
                transform.rotation = physicsTransform.rotation;
        }
    }
}


