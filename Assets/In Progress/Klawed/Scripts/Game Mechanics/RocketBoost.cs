using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RocketBoost : MonoBehaviour
{
    [SerializeField] private float verticalBoostForce = 10f;
    [SerializeField] private float forwardBoostForce = 5f;
    private Rigidbody rootRb;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private GameObject boostParticlesPrefab;
    [SerializeField] private GameObject rocketBoosterObj;
    [SerializeField] private float cooldownDuration = 2f;
    private bool isOnCooldown = false;

    private void Awake()
    {
        RagdollController ragdollController = GetComponent<RagdollController>();
        rootRb = ragdollController.GetPelvis().GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GenericEvent<OnBoostInput>.GetEvent(gameObject.GetInstanceID().ToString()).AddListener(PerformRocketBoost);
    }

    public void PerformRocketBoost()
    {
        //Play rocket boost SFX
        if (isOnCooldown) return;

        AudioManager.Instance?.PlaySFX("RocketBoost");

        // Implement rocket boost logic here
        rootRb.AddForce(Vector3.up * verticalBoostForce, ForceMode.Impulse);
        rootRb.AddForce(centerOfMass.forward * forwardBoostForce, ForceMode.Impulse);
        StartCoroutine(ResetCooldown());
    }

    private IEnumerator ResetCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
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
