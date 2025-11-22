using UnityEngine;

public class Extinguisher : MonoBehaviour {
    public float speed = 10f;
    public float extinguishRate = 0.3f;

    private void Update() {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Burnable burnable)) {
            burnable.ReduceBurn(extinguishRate * Time.deltaTime);
        }
    }
}
