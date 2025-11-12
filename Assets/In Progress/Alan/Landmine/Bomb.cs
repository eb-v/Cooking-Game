using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public GameObject explosionVFX;
    public float lifeAfterExplosion = 1f;

    [Header("Ground Settings")]
    public string groundTag = "Floor";

    [Header("Player Layer")]
    public string playerLayerName = "Player"; 

    [Header("Ignore Layers")]
    public string trainLayerName = "Train";

    [Header("Audio")]
    public AudioClip explosionSfx;
    [Range(0f, 1f)]
    public float explosionVolume = 0.01f;

    private bool hasExploded = false;
    private bool hasLanded = false;

    private AudioSource audioSource;
    private Rigidbody rb;
    private int playerLayer;
    private int trainLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        // cache the player layer index
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        trainLayer  = LayerMask.NameToLayer(trainLayerName);

    }
    private void OnEnable()
    {
        ResetValues();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        Collider other = collision.collider;

        // land on floor
        if (other.CompareTag(groundTag) && !hasLanded)
        {
            hasLanded = true;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true; 
            }

            return;
        }

        // any bodypart on the Player layer touches bomb
        if (other.gameObject.layer == playerLayer)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        
        // 1. Spawn VFX
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }
        
        // play sound
        if (explosionSfx != null)
        {
            audioSource.PlayOneShot(explosionSfx, explosionVolume);
        }
        
        // explosion force
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            // skip the train layer 
            if (hit.gameObject.layer == trainLayer)
                continue;
            
            Rigidbody hitRb = hit.attachedRigidbody;
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                
                // Track player explosions
                if (hit.gameObject.layer == playerLayer)
                {
                    Debug.Log($"Hit player layer object: {hit.gameObject.name}");
                    
                    if (PlayerStatsManager.Instance != null)
                    {
                        PlayerStats playerStats = hit.GetComponentInParent<PlayerStats>();
                        if (playerStats != null)
                        {
                            Debug.Log($"Found PlayerStats with player number: {playerStats.playerNumber}");
                            PlayerStatsManager.Instance.IncrementExplosionsReceived(playerStats.playerNumber);
                        }
                        else
                        {
                            Debug.LogWarning($"No PlayerStats found in parent hierarchy of {hit.gameObject.name}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("PlayerStatsManager.Instance is null!");
                    }
                }
            }
        }
    
    // hide mesh after explosion
    foreach (var rend in GetComponentsInChildren<Renderer>())
    {
        rend.enabled = false;
    }
    
    // return to pool
    StartCoroutine(DestroyAfterExplosion());
}

    private IEnumerator DestroyAfterExplosion()
    {
        yield return new WaitForSeconds(lifeAfterExplosion);
        Destroy(gameObject);
    }

    void ResetValues()
    {
        hasExploded = false;
        hasLanded = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (var rend in GetComponentsInChildren<Renderer>())
        {
            rend.enabled = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
