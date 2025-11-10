using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Airplane : MonoBehaviour
{
    [Header("Movement (units/second)")]
    [Tooltip("Positive values move left/down (we subtract internally).")]
    public float speedX = 3f;   // decreases X each second
    public float speedY = 1.5f; // decreases Y each second

    [Header("Audio")]
    public AudioClip engineLoop;     // assign your plane/woosh clip
    [Range(0f, 1f)] public float volume = 0.7f;
    public bool loop = true;

    [Header("Optional")]
    public bool destroyWhenOffscreen = true;

    AudioSource _audio;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.loop = loop;
        _audio.spatialBlend = 0f; // 2D sound so it stays consistent on screen
        _audio.volume = volume;
        if (engineLoop) _audio.clip = engineLoop;
    }

    void OnEnable()
    {
        if (engineLoop) _audio.Play();
    }

    void Update()
    {
        // Move left/down over time (frame-rate independent)
        float dx = -Mathf.Abs(speedX) * Time.deltaTime;
        float dy = Mathf.Abs(speedY) * Time.deltaTime;
        transform.Translate(new Vector3(dx, dy, 0f), Space.World);
    }

    // Clean up when it leaves the camera view (optional)
    void OnBecameInvisible()
    {
        if (destroyWhenOffscreen) Destroy(gameObject);
    }
}
