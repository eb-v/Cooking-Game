using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float speed = 2f;          
    [SerializeField] private float materialSpeed = 1f;  
    [SerializeField] private Vector3 direction = Vector3.forward; 
    [SerializeField] private List<GameObject> onBelt;    
    private bool isRunning = true;

    private Material material;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        //if (material != null)
        //    material.mainTextureOffset += new Vector2(0, 1) * materialSpeed * Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isRunning)
            RunConveyorBelt();
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

    public void ChangeDirection(int inputDirection)
    {
        if (inputDirection == 1)
        {
            direction.x = Mathf.Abs(direction.x);
            direction.y = Mathf.Abs(direction.y);
            direction.z = Mathf.Abs(direction.z);
        }
        else if (inputDirection == -1)
        {
            direction.x = -Mathf.Abs(direction.x);
            direction.y = -Mathf.Abs(direction.y);
            direction.z = -Mathf.Abs(direction.z);
        }
        
    }

    private void RunConveyorBelt()
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

    public void ChangeRunningStatus()
    {
        isRunning = !isRunning;
    }
}
