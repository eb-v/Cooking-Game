using UnityEngine;

public class FireAudioManager : MonoBehaviour
{
    public static FireAudioManager Instance { get; private set; }

    [Header("Looping Fire Audio")]
    [SerializeField] private AudioSource fireLoopSource;   // assign in Inspector, looping
    [SerializeField] private bool playOnStartIfFiresExist = false;

    private int activeFireCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (fireLoopSource != null)
        {
            fireLoopSource.loop = true;
            fireLoopSource.playOnAwake = false;
            fireLoopSource.Stop();
        }
    }

    public void RegisterFire()
    {
        activeFireCount++;
        if (activeFireCount == 1)
            StartLoop();
    }

    public void UnregisterFire()
    {
        activeFireCount = Mathf.Max(0, activeFireCount - 1);
        if (activeFireCount == 0)
            StopLoop();
    }

    private void StartLoop()
    {
        if (fireLoopSource != null && !fireLoopSource.isPlaying)
            fireLoopSource.Play();
    }

    private void StopLoop()
    {
        if (fireLoopSource != null && fireLoopSource.isPlaying)
            fireLoopSource.Stop();
    }
}
