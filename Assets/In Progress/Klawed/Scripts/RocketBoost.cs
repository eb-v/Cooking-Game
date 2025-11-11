using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketBoost : MonoBehaviour
{
    [SerializeField] private float verticalBoostForce = 10f;
    [SerializeField] private float forwardBoostForce = 5f;
    private Rigidbody rootRb;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private GameObject boostParticlesPrefab;
    [SerializeField] private GameObject rocketBoosterObj;

    private void Awake()
    {
        RagdollController ragdollController = GetComponent<RagdollController>();
        rootRb = ragdollController.GetPelvis().GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GenericEvent<OnBoostInput>.GetEvent(gameObject.name).AddListener(PerformRocketBoost);
    }

    private void PerformRocketBoost()
    {
        // Implement rocket boost logic here
        rootRb.AddForce(Vector3.up * verticalBoostForce, ForceMode.Impulse);
        rootRb.AddForce(centerOfMass.forward * forwardBoostForce, ForceMode.Impulse);
        
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Level 2")
        {
            if (rocketBoosterObj.activeSelf) return;
            rocketBoosterObj.SetActive(true);
        }
    }
}
