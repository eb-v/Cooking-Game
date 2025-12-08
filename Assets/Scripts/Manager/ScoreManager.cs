using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [ReadOnly]
    [SerializeField] private int currentScore = 0;
    [SerializeField] private float multiplier = 1.0f;

    private static ScoreManager instance;

    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ScoreManager>();
            }
            return instance;
        }
    }

    public void ChangeScore(int amount, GameObject player)
    {
        int scoreAdded = Mathf.RoundToInt(amount * multiplier);
        currentScore += scoreAdded;

        GenericEvent<UpdateScoreDisplayEvent>.GetEvent("UpdateScoreDisplayEvent").Invoke(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
        GenericEvent<UpdateScoreDisplayEvent>.GetEvent("UpdateScoreDisplayEvent").Invoke(currentScore);
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}
