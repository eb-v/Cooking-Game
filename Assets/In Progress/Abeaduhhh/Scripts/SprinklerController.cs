using UnityEngine;

public class SprinklerController : MonoBehaviour {
    [Header("Button Reference")]
    public SprinklerButton buttonA;

    //[Header("Sprinkler Components")]
    //[Tooltip("Optional reference to a pre-assigned water effect. If left empty, the system will spawn one from the pool.")]
    //public ParticleSystem sprinklerEffect;

    [Header("Pooling Settings")]
    [Tooltip("Prefab reference for the water effect that should be spawned from the pool.")]
    public GameObject waterEffectPrefab;

    [Tooltip("If true, returns this sprinkler to the pool when deactivated.")]
    public bool autoReturnToPool = false;

    [Header("Sprinkler Settings")]
    [Tooltip("How long the sprinkler stays active before turning off automatically.")]
    public float sprinklerDuration = 5f;

    private bool buttonAActive;
    //private bool buttonBActive;
    private bool sprinklerActive;

    // internal reference to the spawned particle effect
    private ParticleSystem sprinklerEffect;

    private void Start() { // listening for button updates, this is where i will add the 2nd button for the dual interactions
        if (buttonA != null)
            buttonA.OnButtonStateChanged.AddListener(OnButtonAChanged);
    }

    private void OnButtonAChanged(bool pressed) {
        buttonAActive = pressed;
        CheckActivation();
    }

    //private void OnButtonBChanged(bool pressed) {
    //    buttonBActive = pressed;
    //    CheckActivation();
    //}

    private void CheckActivation() {
        bool shouldActivate = buttonAActive;
        //bool shouldActivate = buttonAActive && buttonBActive;

        if (shouldActivate && !sprinklerActive)
            StartSprinkler();
        else if (!shouldActivate && sprinklerActive)
            StopSprinkler();
    }

    public void StartSprinkler() {
        if (sprinklerActive) return;
        sprinklerActive = true;

        // always spawn from pool when activating
        if (waterEffectPrefab != null) {
            GameObject pooledEffect = ObjectPoolManager.SpawnObject(
                waterEffectPrefab,
                transform.position,
                transform.rotation
            );
            sprinklerEffect = pooledEffect.GetComponent<ParticleSystem>();

            if (sprinklerEffect == null) {
                Debug.LogError($"{name}: The waterEffectPrefab is missing a ParticleSystem component!");
                return;
            }
        } else {
            Debug.LogError($"{name}: No waterEffectPrefab assigned to SprinklerController!");
            return;
        }

        if (sprinklerEffect != null && !sprinklerEffect.isPlaying)
            sprinklerEffect.Play();

        // automatically turn off after sprinklerDuration seconds
        if (sprinklerDuration > 0)
            Invoke(nameof(StopSprinkler), sprinklerDuration);

        Debug.Log($"{name}: Sprinkler Activated for {sprinklerDuration} seconds");
    }

    public void StopSprinkler() {
        if (!sprinklerActive) return;
        sprinklerActive = false;

        if (sprinklerEffect != null) {
            sprinklerEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // safely return the effect to the pool if it came from one
            if (ObjectPoolManager.IsPooledObject(sprinklerEffect.gameObject)) {
                ObjectPoolManager.ReturnObjectToPool(sprinklerEffect.gameObject);
                Debug.Log($"{name}: Sprinkler Effect Returned to Pool");
            } else {
                Destroy(sprinklerEffect.gameObject);
                Debug.LogWarning($"{name}: Sprinkler Effect was not pooled — destroyed instead.");
            }

            sprinklerEffect = null;
        }

        CancelInvoke(nameof(StopSprinkler));

        if (autoReturnToPool && ObjectPoolManager.IsPooledObject(gameObject)) {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
            Debug.Log($"{name}: Sprinkler Returned to Pool");
        }

        Debug.Log($"{name}: Sprinkler Deactivated");
    }

    public void ResetSprinkler() {
        sprinklerActive = false;

        if (sprinklerEffect != null) {
            sprinklerEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (ObjectPoolManager.IsPooledObject(sprinklerEffect.gameObject)) {
                ObjectPoolManager.ReturnObjectToPool(sprinklerEffect.gameObject);
            } else {
                Destroy(sprinklerEffect.gameObject);
            }
            sprinklerEffect = null;
        }

        CancelInvoke(nameof(StopSprinkler));
    }
}
