using System;                // for EventHandler, EventArgs
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    private enum State { WaitingToStart, CountdownToStart, GamePlaying, GameOver }
    private State state;

    private float waitingToStartDuration = 1f;
    private float countdownToStartDuration = 3f;
    private float gamePlayingDuration;

    private float gamePlayingTimerMax = 10f; // test value
    

    private float waitingToStartTimer;
    private float countdownToStartTimer;
    private float gamePlayingTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        state = State.WaitingToStart;

        waitingToStartTimer = waitingToStartDuration;
        countdownToStartTimer = countdownToStartDuration;
        gamePlayingTimer = gamePlayingDuration;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;                 // <-- subtract
                if (waitingToStartTimer <= 0f)
                {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer <= 0f)
                {
                    countdownToStartTimer = 0f;
                    state = State.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0f)
                {
                    gamePlayingTimer = 0f;
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.GameOver:
                break;
        }
    }

    public bool IsGamePlaying() => state == State.GamePlaying;
    public bool IsCountdownToStartActive() => state == State.CountdownToStart;
    public float GetCountdownToStartTimer() => Mathf.Max(0f, countdownToStartTimer);

    public float GetGamePlayingTimeLeft() => Mathf.Max(0f, gamePlayingTimer);
    public float GetGamePlayingDuration() => gamePlayingDuration;

    public float GetGamelayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer / gamePlayingTimerMax);
    }

}
