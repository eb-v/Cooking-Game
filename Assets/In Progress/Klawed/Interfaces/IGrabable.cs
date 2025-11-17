using UnityEngine;

public interface IGrabable
{
    bool isGrabbed { get; set; }
    GrabData grabData { get; set; }
    GameObject player { get; set; }

    void GrabObject(GameObject player);
}
