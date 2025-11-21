using UnityEngine;

public interface IGrabable
{
    bool isGrabbed { get; set; }
    GrabData grabData { get; set; }
    GameObject currentPlayer { get; set; }
    GameObject GetGameObject();

    void GrabObject(GameObject player);

    void ReleaseObject(GameObject player);

    void ThrowObject(GameObject player);
}
