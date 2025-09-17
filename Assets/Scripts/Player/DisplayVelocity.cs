using UnityEngine;

[System.Serializable]
public class DisplayVelocity : MonoBehaviour
{
    public Vector3 velocity;

    void Update()
    {
        velocity = GetComponent<Rigidbody>().linearVelocity;
    }
}
