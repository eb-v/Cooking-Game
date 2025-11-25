using UnityEngine;

[CreateAssetMenu(fileName = "Score System", menuName = "Systems/Score System")]
public class ScoreSystem : ScriptableObject
{
    [SerializeField] private static int currentScore = 0;
    [SerializeField] private static float multiplier = 1.0f;   
    
    private static ScoreSystem instance;
    
    public static ScoreSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ScoreSystem>("Systems/Score System");
            }
            return instance;
        }
    }
    
    public static void ChangeScore(int amount, GameObject player)
    {
        int scoreAdded = Mathf.RoundToInt(amount * multiplier);
        currentScore += scoreAdded;
        
        // Track via PlayerStatsManager using player number
        
        PlayerStatsManager.AddPoints(player, scoreAdded);
        
        
        GenericEvent<UpdateScoreDisplayEvent>.GetEvent("UpdateScoreDisplayEvent").Invoke(currentScore);
    }
    
    public static void ResetScore()
    {
        currentScore = 0;
        GenericEvent<UpdateScoreDisplayEvent>.GetEvent("UpdateScoreDisplayEvent").Invoke(currentScore);
    }
    
    public static int GetCurrentScore()
    {
        return currentScore;
    }
}