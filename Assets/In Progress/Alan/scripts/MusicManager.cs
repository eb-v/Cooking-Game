using UnityEngine;
using UnityEngine.Audio;

public enum MusicTrack
{
    MainMenu,
    Pregame,
    Level1,
    Level2,

    AwardsScene
}

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Mixer (optional but recommended)")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;  // "main menu" file
    [SerializeField] private AudioClip pregameMusic;   // music3
    [SerializeField] private AudioClip level1Music;    // music2
    [SerializeField] private AudioClip level2Music;    // music4
    [SerializeField] private AudioClip awardSceneMusic;    // music4


    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f; // 2D music

        if (musicMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = musicMixerGroup;
        }
    }

    public void PlayTrack(MusicTrack track)
    {
        AudioClip clip = null;

        switch (track)
        {
            case MusicTrack.MainMenu: clip = mainMenuMusic; break;
            case MusicTrack.Pregame:  clip = pregameMusic;  break;
            case MusicTrack.Level1:   clip = level1Music;   break;
            case MusicTrack.Level2:   clip = level2Music;   break;
            case MusicTrack.AwardsScene:   clip = awardSceneMusic;   break;

        }

        if (clip == null)
        {
            Debug.LogWarning($"[MusicManager] No clip assigned for {track}");
            return;
        }

        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
