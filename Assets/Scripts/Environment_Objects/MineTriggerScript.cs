using UnityEngine;

public class MineTriggerScript : MonoBehaviour
{
    [SerializeField] private float explosionForce = 500f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            GenericEvent<OnExplosionTriggered>.GetEvent("Player 1").Invoke(gameObject.transform.position, explosionForce);
        }
        else if (other.transform.name == "Test Cube")
        {
            GenericEvent<OnExplosionTriggered>.GetEvent("Test Cube").Invoke(gameObject.transform.position, explosionForce);
        }
        
    }
}
