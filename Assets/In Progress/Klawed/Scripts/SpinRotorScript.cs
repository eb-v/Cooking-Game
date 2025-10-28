using UnityEngine;

public class SpinRotorScript : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 5f;   

    private void FixedUpdate()
    {
        SpinRotor();
    }

    private void SpinRotor()
    {
        transform.Rotate(0f, 0f, spinSpeed);
    }
}
