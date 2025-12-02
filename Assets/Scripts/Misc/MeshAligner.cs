using UnityEngine;

public class MeshAligner : MonoBehaviour
{
    [SerializeField] private GameObject bottomObject;
    [SerializeField] private GameObject topObject;


    public void PlaceOnTop()
    {
        MeshRenderer bottomRenderer = bottomObject.GetComponentInChildren<MeshRenderer>();
        MeshRenderer topRenderer = topObject.GetComponentInChildren<MeshRenderer>();

        if (!bottomRenderer || !topRenderer)
        {
            Debug.LogError("One or both objects are missing MeshRenderers.");
            return;
        }

        // Get world bounds
        Bounds bottomBounds = bottomRenderer.bounds;
        Bounds topBounds = topRenderer.bounds;

        // Compute new position for the top object
        Vector3 newTopPos = topObject.transform.position;

        float bottomTopY = bottomBounds.max.y; // highest point of bottom object
        float offset = topBounds.extents.y;    // distance from center to bottom of the top object

        // Place so bottom of the top object touches top of the bottom object
        newTopPos.y = bottomTopY;

        topObject.transform.position = newTopPos;

        Debug.Log("Top object placed on bottom successfully.");
    }
}
