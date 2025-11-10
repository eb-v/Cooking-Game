using UnityEngine;
using TMPro;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float countdownDuration = 3f;
    [SerializeField] private float fadeDuration = 0.3f;
    
    private float countdownTimer;
    private bool isCountingDown = false;
    private int currentNumber;
    private bool isFading = false;
    private float fadeTimer;
    private Color textColor;
    
    // Track if we set the timeScale to 0 (vs pause menu doing it)
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
        
        textColor = countdownText.color;
        countdownTimer = countdownDuration;
        currentNumber = Mathf.CeilToInt(countdownDuration);
        countdownText.text = currentNumber.ToString();
        isCountingDown = true;
        gameObject.SetActive(true);
        
        // Pause for countdown
        Time.timeScale = 0f;
        CountdownIsActive = true;
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
                Time.timeScale = 1f;
                isCountingDown = false;
                CountdownIsActive = false;
                gameObject.SetActive(false);
                
                Debug.Log("Countdown finished! Time.timeScale set to 1. Timer starts NOW.");
            }
        }
    }

    private void OnDestroy()
    {
        CountdownIsActive = false;
        CountdownIsPaused = false;
    }
}