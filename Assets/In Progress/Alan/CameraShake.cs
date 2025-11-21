using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Default Shake Settings")]
    [SerializeField] private float defaultDuration = 0.8f;
    [SerializeField] private float defaultAmplitude = 1.5f;
    [SerializeField] private float defaultFrequency = 2f;

    [Header("Noise Settings")]
    [SerializeField] private NoiseSettings noiseProfile;

    private CinemachineCamera cmCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine currentShake;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cmCamera = GetComponent<CinemachineCamera>();

        noise = GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            noise = gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
        }

        if (noiseProfile != null)
        {
            noise.NoiseProfile = noiseProfile;
        }

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }

    private void OnDisable()
    {
        if (noise != null)
        {
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 0f;
        }
    }

    public void Shake()
    {
        Shake(defaultDuration, defaultAmplitude, defaultFrequency);
    }

    public void Shake(float duration, float amplitude)
    {
        Shake(duration, amplitude, defaultFrequency);
    }

    public void Shake(float duration, float amplitude, float frequency)
    {
        if (currentShake != null)
            StopCoroutine(currentShake);

        currentShake = StartCoroutine(ShakeRoutine(duration, amplitude, frequency));
    }

    private IEnumerator ShakeRoutine(float duration, float amplitude, float frequency)
    {
        float elapsed = 0f;

        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
        currentShake = null;
    }
}
