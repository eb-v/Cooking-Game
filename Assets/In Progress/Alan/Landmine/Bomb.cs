using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public GameObject explosionVFX;  // optional, can leave empty
    public float lifeAfterExplosion = 1f;
    public float fuseTime = 2f;      // how long it waits on the ground

    [Header("Ground Settings")]
    public string groundTag = "Floor";   // tag for your floor tiles

    [Header("Audio")]
    public AudioClip explosionSfx;
    [Range(0f, 1f)]
    public float explosionVolume = 0.3f;  // 0.3 = 30% volume
    private bool isTriggered = false;
    private bool hasExploded = false;
    private AudioSource audioSource;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTriggered || hasExploded) return;

        // Only start fuse if we hit the floor tiles
        if (collision.collider.CompareTag(groundTag))
        {
            StartCoroutine(FuseRoutine());
        }
    }

    private IEnumerator FuseRoutine()
    {
        isTriggered = true;

        // Stop moving and stay on the tile
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;  // no more physics movement
        }

        // Wait a few seconds before exploding
        yield return new WaitForSeconds(fuseTime);

        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // 1. Spawn VFX (optional)
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        // 2. Play SFX
        if (explosionSfx != null)
        {
            audioSource.PlayOneShot(explosionSfx, explosionVolume);
        }

        

        // 3. Apply explosion force
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            Rigidbody hitRb = hit.attachedRigidbody;
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // 4. Hide the mesh after explosion
        foreach (var rend in GetComponentsInChildren<Renderer>())
        {
            rend.enabled = false;
        }

        // 5. Destroy after sound finishes
        Destroy(gameObject, lifeAfterExplosion);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
