using Unity.VisualScripting;
using UnityEngine;

public class TestCube : MonoBehaviour, IKnockBack
{
    private void Awake()
    {
        GenericEvent<OnExplosionTriggered>.GetEvent("Test Cube").AddListener(OnKnockBack);
    }

    public void OnKnockBack(Vector3 forceOriginPos, float forceAmount)
    {
        forceAmount = 30f;
        Vector3 direction = (gameObject.transform.position - forceOriginPos).normalized;
        gameObject.GetComponent<Rigidbody>().AddForce(direction * forceAmount, ForceMode.Impulse);
    }
}
