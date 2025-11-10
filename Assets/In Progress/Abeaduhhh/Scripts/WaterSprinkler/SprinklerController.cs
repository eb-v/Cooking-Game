using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SprinklerController : MonoBehaviour {
    [Header("Button Reference")]
    public SprinklerButton buttonA;
    //public SprinklerButton buttonB;

    [Header("Pooling Settings")]
    public GameObject waterEffectPrefab;
    public bool autoReturnToPool = false;

    [Header("Sprinkler Settings")]
    [Tooltip("How long the sprinkler stays active before turning off automatically.")]
    public float sprinklerDuration = 5f;

    [Tooltip("How long after turning on before it starts putting out fires.")]
    public float extinguishDelay = 2f;

    [Tooltip("Radius within which fires can be extinguished.")]
    public float extinguishRadius = 20f;

    [Tooltip("How often (seconds) to check for fires to extinguish while active.")]
    public float extinguishCheckInterval = 0.75f;

    private bool sprinklerActive;
    private ParticleSystem sprinklerEffect;
    private GameObject sprinklerEffectInstance;

    private static readonly Collider[] overlapResults = new Collider[64];

    private void Awake() {
        // Subscribe to the global Start/Stop Sprinkler events
        GenericEvent<StartSprinklerEvent>.GetEvent("SprinklerController").AddListener(OnStartSprinklerEvent);
        GenericEvent<StopSprinklerEvent>.GetEvent("SprinklerController").AddListener(OnStopSprinklerEvent);
    }

    private void OnDestroy() {
        // Always clean up listeners
        GenericEvent<StartSprinklerEvent>.GetEvent("SprinklerController").RemoveListener(OnStartSprinklerEvent);
        GenericEvent<StopSprinklerEvent>.GetEvent("SprinklerController").RemoveListener(OnStopSprinklerEvent);
    }

    private void Start() {
        if (buttonA != null)
            buttonA.OnButtonStateChanged.AddListener(OnButtonAChanged);

        //if(buttonB != null)
        //    buttonB.OnButtonStateChanged.AddListener(OnButtonBChanged);

        if (waterEffectPrefab != null) {
            if (ObjectPoolManager.IsPooledObject(waterEffectPrefab)) {
                sprinklerEffectInstance = ObjectPoolManager.SpawnObject(waterEffectPrefab, transform.position, transform.rotation);
            } else {
                sprinklerEffectInstance = Instantiate(waterEffectPrefab, transform.position, transform.rotation);
            }

            sprinklerEffect = sprinklerEffectInstance.GetComponent<ParticleSystem>();

            if (sprinklerEffectInstance != null)
                sprinklerEffectInstance.SetActive(false); // disable initially
        }
    }

    private void OnButtonAChanged(bool pressed) {
        if (pressed)
            GenericEvent<StartSprinklerEvent>.GetEvent("SprinklerController").Invoke();
    }

    //private void OnButtonBChanged(bool pressed) {
    //    if (pressed)
    //        GenericEvent<StartSprinklerEvent>.GetEvent("SprinklerController").Invoke();
    //}

    private void OnStartSprinklerEvent() => StartSprinkler();
    private void OnStopSprinklerEvent() => StopSprinkler();

    public void StartSprinkler() {
        if (sprinklerActive) return;
        sprinklerActive = true;

        if (sprinklerEffectInstance != null) {
            sprinklerEffectInstance.SetActive(true);
            if (!sprinklerEffect.isPlaying)
                sprinklerEffect.Play();
        }

        InvokeRepeating(nameof(CheckForFires), extinguishDelay, extinguishCheckInterval);

        if (sprinklerDuration > 0)
            StartCoroutine(AutoStopSprinkler());

        Debug.Log($"{name}: Sprinkler Activated for {sprinklerDuration} seconds (fires extinguish after {extinguishDelay}s)");
    }

    private IEnumerator AutoStopSprinkler() {
        yield return new WaitForSeconds(sprinklerDuration);
        GenericEvent<StopSprinklerEvent>.GetEvent("SprinklerController").Invoke();
    }

    private void CheckForFires() {
        int count = Physics.OverlapSphereNonAlloc(transform.position, extinguishRadius, overlapResults);
        for (int i = 0; i < count; i++) {
            FireController fire = overlapResults[i].GetComponentInParent<FireController>();
            if (fire != null) {
                fire.StopFireImmediate();
            }
        }
    }

    public void StopSprinkler() {
        if (!sprinklerActive) return;
        sprinklerActive = false;

        CancelInvoke(nameof(CheckForFires));
        CancelInvoke(nameof(StopSprinkler));

        if (sprinklerEffect != null) {
            sprinklerEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (ObjectPoolManager.IsPooledObject(sprinklerEffectInstance)) {
                StartCoroutine(ReturnEffectNextFrame(sprinklerEffectInstance.GetComponent<ParticleSystem>()));
            } else {
                sprinklerEffectInstance.SetActive(false);
            }
        }

        Debug.Log($"{name}: Sprinkler Deactivated");
    }

    private IEnumerator ReturnEffectNextFrame(ParticleSystem ps) {
        yield return null;

        if (ps != null) {
            if (ObjectPoolManager.IsPooledObject(ps.gameObject)) {
                ObjectPoolManager.ReturnObjectToPool(ps.gameObject);
                Debug.Log($"{name}: Sprinkler Effect Returned to Pool");
            } else {
                ps.gameObject.SetActive(false);
                Debug.LogWarning($"{name}: Sprinkler Effect was not pooled — just disabled instead.");
            }
        }
    }

    public void ResetSprinkler() {
        sprinklerActive = false;
        CancelInvoke(nameof(CheckForFires));
        CancelInvoke(nameof(StopSprinkler));

        if (sprinklerEffect != null) {
            sprinklerEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (sprinklerEffectInstance != null) {
            sprinklerEffectInstance.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, extinguishRadius);
    }
}
