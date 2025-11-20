using UnityEngine;

public class SfxTestOnStart : MonoBehaviour
{
    private void Start()
    {
        // Use a name that you know is in AudioManager's list
        AudioManager.Instance?.PlaySFX("StartCountdown");
    }
}
