using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ConeGizmo : MonoBehaviour
{
    [SerializeField] private float distance = 10f;
    [SerializeField] private float angle = 30f;
    [SerializeField] private Vector3 posOffset;
    [SerializeField] private float directionAngle = 0f;
    [SerializeField] private Vector3 forwardDirection;

    [SerializeField] private LayerMask layerMask;

    private Vector3 distanceDirection => forwardDirection.normalized * distance;

    private float coneAngle => angle * 2f;

    private void OnDrawGizmos()
    {
        

        //// Forward line
        //Gizmos.color = Color.green; // Forward direction
        //Gizmos.DrawRay(transform.position + posOffset, transform.forward * distance);

        //// Left line
        //Gizmos.color = Color.red; // Left edge
        //Quaternion leftRot = Quaternion.AngleAxis(-angle, Vector3.up);
        //Vector3 leftDir = leftRot * transform.forward * distance;
        //Gizmos.DrawRay(transform.position + posOffset, leftDir);

        //// Right line
        //Gizmos.color = Color.blue; // Right edge
        //Quaternion rightRot = Quaternion.AngleAxis(angle, Vector3.up);
        //Vector3 rightDir = rightRot * transform.forward * distance;
        //Gizmos.DrawRay(transform.position + posOffset, rightDir);

        //// Center / other (if needed)
        //Gizmos.color = Color.yellow;


        // draw sphere collider
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetCenterPoint(), distance / 2f);

    }

    public Vector3 GetCenterPoint()
    {
        return transform.position + posOffset + transform.forward * distance / 2f;
    }

    public List<GameObject> GetObjectsInSphere()
    {
        List<GameObject> objectsInSphere = new List<GameObject>();
        Vector3 origin = GetCenterPoint();

        Collider[] hit = Physics.OverlapSphere(origin, distance / 2f, layerMask);

        foreach (Collider col in hit)
        {
            GameObject rootObj = col.transform.root.gameObject;
            if (objectsInSphere.Contains(rootObj))
            {
                continue;
            }
            objectsInSphere.Add(rootObj);
        }

        return objectsInSphere;
    }


}
