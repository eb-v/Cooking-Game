using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float speed = 2f;          
    [SerializeField] private float materialSpeed = 1f;  
    [SerializeField] private Vector3 direction = Vector3.forward; 
    [SerializeField] private List<GameObject> onBelt;    

    private Material material;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (material != null)
            material.mainTextureOffset += new Vector2(0, 1) * materialSpeed * Time.deltaTime;
    }

    void FixedUpdate()
    {
        for (int i = onBelt.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = onBelt[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 newPosition = rb.position + direction.normalized * speed * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);
            }
            else
            {
                onBelt.RemoveAt(i);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!onBelt.Contains(collision.gameObject))
            onBelt.Add(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        onBelt.Remove(collision.gameObject);
    }
}
