using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SfxSound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(-3f, 3f)] public float pitch = 1f;
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<SfxSound> sfxSounds = new List<SfxSound>();

    private Dictionary<string, SfxSound> sfxLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("[AudioManager] Duplicate instance on " + gameObject.name + ", destroying.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource == null)
            Debug.LogError("[AudioManager] No AudioSource assigned or found!");
        else
        {
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f; // 2D
        }

        BuildLookup();
        Debug.Log("[AudioManager] Initialized with " + sfxSounds.Count + " SFX.");
    }

    private void BuildLookup()
    {
        sfxLookup = new Dictionary<string, SfxSound>(StringComparer.OrdinalIgnoreCase);

        foreach (var s in sfxSounds)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.name) || s.clip == null)
            {
                Debug.LogWarning("[AudioManager] Skipping SFX with missing name/clip.");
                continue;
            }

            if (sfxLookup.ContainsKey(s.name))
            {
                Debug.LogWarning("[AudioManager] Duplicate SFX name '" + s.name + "', keeping first.");
                continue;
            }

            sfxLookup.Add(s.name, s);
        }
    }

    public void PlaySFX(string name)
{
    if (!sfxLookup.TryGetValue(name, out var sound) || sound.clip == null)
    {
        Debug.LogWarning("[AudioManager] No SFX found with name '" + name + "'.");
        return;
    }

    // Position = at listener (camera); if no camera, just at origin
    Vector3 pos = Vector3.zero;
    if (Camera.main != null)
        pos = Camera.main.transform.position;

    // EXACTLY like RawAudioTest: create a one-shot AudioSource at that position
    AudioSource.PlayClipAtPoint(sound.clip, pos, sound.volume);

    Debug.Log("[AudioManager] PlayClipAtPoint SFX '" + name + "'.");
}


}
