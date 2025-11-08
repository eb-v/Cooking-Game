using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] ScoreCounterUIScript scoreCounterUI;
    [SerializeField] private int currentScore = 0;
    [SerializeField] private float multiplier = 1.0f;   

    private void Awake()
    {
        GenericEvent<ScoreChangedEvent>.GetEvent("ScoreChangedEvent").AddListener(ChangeScore);
    }


    private void ChangeScore(int amount)
    {
        amount = Mathf.RoundToInt(amount * multiplier);
        currentScore += amount;
        scoreCounterUI.UpdateDisplay(currentScore);
    }




}
