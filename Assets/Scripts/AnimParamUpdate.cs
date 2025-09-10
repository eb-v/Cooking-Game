using UnityEngine;
using System;

public class AnimParamUpdate : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = transform.root.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        Vector3 vel = rb.linearVelocity;
        float horizontalSpeed = new Vector2(vel.x, vel.z).magnitude;
        animator.SetFloat("Speed", horizontalSpeed);
    }
}
