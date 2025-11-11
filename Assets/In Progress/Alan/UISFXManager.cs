using UnityEngine;

public class UISFXManager : MonoBehaviour
{
    public static UISFXManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;

    [Header("Buttons")]
    [SerializeField] private AudioClip buttonHoverSFX;
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip buttonBackSFX;

    [Header("Panels")]
    [SerializeField] private AudioClip panelOpenSFX;
    [SerializeField] private AudioClip panelCloseSFX;

    [Header("Game States")]
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioClip awardsSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonHover()  { PlayOneShot(buttonHoverSFX); }
    public void PlayButtonClick()  { PlayOneShot(buttonClickSFX); }
    public void PlayButtonBack()   { PlayOneShot(buttonBackSFX); }

    public void PlayPanelOpen()    { PlayOneShot(panelOpenSFX); }
    public void PlayPanelClose()   { PlayOneShot(panelCloseSFX); }

    public void PlayGameOver()     { PlayOneShot(gameOverSFX); }
    public void PlayAwards()       { PlayOneShot(awardsSFX); }

    private void PlayOneShot(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
