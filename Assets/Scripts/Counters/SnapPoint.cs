using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SnapPoint : MonoBehaviour
{
    [HideInInspector] 
    public Transform snapPos;

    private void Awake()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (snapPos == null)
        {
            GameObject snapGO = new GameObject("SnapPos");
            snapGO.transform.parent = transform;

            Vector3 topCenter = box.center + new Vector3(0, box.size.y / 2f + 0.05f, 0);
            snapGO.transform.localPosition = topCenter;
            snapGO.transform.localRotation = Quaternion.identity;
            snapGO.transform.localScale = Vector3.one;

            snapPos = snapGO.transform;
        }

        Debug.Log($"SnapPoint created at {snapPos.position}");
    }

    public Transform GetSnapPos()
    {
        return snapPos;
    }
}
