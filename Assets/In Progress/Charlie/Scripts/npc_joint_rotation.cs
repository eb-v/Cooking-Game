using UnityEngine;

public class npc_joint_rotation : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis = new Vector3(0, 0, 1); 
    [SerializeField] private float maxDegrees = 45f; 
    [SerializeField] private float speed = 1f; 

    private Quaternion startRotation;
    private float time = 0f;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {

        time += Time.deltaTime * speed;
        float angle = Mathf.Sin(time) * maxDegrees;
        transform.localRotation = startRotation * Quaternion.Euler(rotationAxis * angle);
    }
}