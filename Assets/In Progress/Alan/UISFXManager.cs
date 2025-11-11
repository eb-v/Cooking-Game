using UnityEngine;

public class UISFXManager : MonoBehaviour
{
    public static UISFXManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Buttons / Menus")]
    [SerializeField] private AudioClip buttonHoverClip;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip pauseOpenClip;
    [SerializeField] private AudioClip pauseCloseClip;

    [Header("Gameplay UI")]
    [SerializeField] private AudioClip countdownTickClip;
    [SerializeField] private AudioClip countdownFinalClip;
    [SerializeField] private AudioClip timerAlmostDoneClip;
    [SerializeField] private AudioClip scoreGainClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip awardsClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }

    // Public methods other scripts will call:
    public void PlayButtonHover()      => PlayClip(buttonHoverClip);
    public void PlayButtonClick()      => PlayClip(buttonClickClip);
    public void PlayPauseOpen()        => PlayClip(pauseOpenClip);
    public void PlayPauseClose()       => PlayClip(pauseCloseClip);
    public void PlayCountdownTick()    => PlayClip(countdownTickClip);
    public void PlayCountdownFinal()   => PlayClip(countdownFinalClip);
    public void PlayTimerAlmostDone()  => PlayClip(timerAlmostDoneClip);
    public void PlayScoreGain()        => PlayClip(scoreGainClip);
    public void PlayGameOver()         => PlayClip(gameOverClip);
    public void PlayAwards()           => PlayClip(awardsClip);
}
