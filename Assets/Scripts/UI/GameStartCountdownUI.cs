using UnityEngine;
using TMPro;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float countdownDuration = 3f;
    [SerializeField] private float fadeDuration = 0.3f;

    [SerializeField] private AudioSource audioSource;   
    [SerializeField] private AudioClip tickSFX;         
    [SerializeField] private AudioClip finalSFX;       
    
    private float countdownTimer;
    private bool isCountingDown = false;
    private int currentNumber;
    private bool isFading = false;
    private float fadeTimer;
    private Color textColor;

    private bool usingCombinedCountdownSFX = false;
    
    public static bool CountdownIsActive { get; private set; }
    public static bool CountdownIsPaused { get; set; }

    private void Awake()
    {
        CountdownIsActive = false;
        CountdownIsPaused = false;
    }

    private void Start()
    {
        if (countdownText == null) countdownText = GetComponent<TMP_Text>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        
        textColor = countdownText.color;
        countdownTimer = countdownDuration;
        currentNumber = Mathf.CeilToInt(countdownDuration);
        countdownText.text = currentNumber.ToString();
        isCountingDown = true;
        gameObject.SetActive(true);
        
        // Pause for countdown
        Time.timeScale = 0f;
        CountdownIsActive = true;

        // prefer from AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("StartCountdown");
            usingCombinedCountdownSFX = true;
        }
        else
        {
            usingCombinedCountdownSFX = false; // fallback instead
        }
    }

    private void Update()
    {
        if (isCountingDown && !CountdownIsPaused)
        {
            // Use unscaled time so countdown works
            countdownTimer -= Time.unscaledDeltaTime;
            
            int newNumber = Mathf.CeilToInt(countdownTimer);
            
            // Check if we need to change to a new number
            if (newNumber != currentNumber && newNumber > 0)
            {
                if (!isFading)
                {
                    if (!usingCombinedCountdownSFX)
                        PlayTickSFX();

                    isFading = true;
                    fadeTimer = fadeDuration;
                }
            }
            
            // Handle fade animation
            if (isFading)
            {
                fadeTimer -= Time.unscaledDeltaTime;
                
                if (fadeTimer > fadeDuration / 2f)
                {
                    // Fade out
                    float alpha = (fadeTimer - fadeDuration / 2f) / (fadeDuration / 2f);
                    countdownText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
                }
                else if (fadeTimer > 0f)
                {
                    // Change number and fade in
                    if (currentNumber != newNumber)
                    {
                        currentNumber = newNumber;
                        countdownText.text = currentNumber.ToString();
                    }
                    
                    float alpha = 1f - (fadeTimer / (fadeDuration / 2f));
                    countdownText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
                }
                else
                {
                    isFading = false;
                    countdownText.color = textColor;
                }
            }

            // Countdown finished
            if (countdownTimer <= 0f)
            {
                // Only play final SFX if we're in fallback mode
                if (!usingCombinedCountdownSFX)
                    PlayFinalSFX();
                
                Time.timeScale = 1f;
                isCountingDown = false;
                CountdownIsActive = false;
                countdownText.enabled = false;
                
                Debug.Log("Countdown finished! Time.timeScale set to 1. Timer starts NOW.");
            }
        }
    }

    private void OnDestroy()
    {
        CountdownIsActive = false;
        CountdownIsPaused = false;
    }

    private void PlayTickSFX()
    {
        // Fallback local tick sound
        if (audioSource != null && tickSFX != null)
        {
            audioSource.PlayOneShot(tickSFX);
        }
    }

    private void PlayFinalSFX()
    {
        // Fallback local final sound
        if (audioSource != null && finalSFX != null)
        {
            audioSource.PlayOneShot(finalSFX);
        }
    }
}
