using UnityEngine;

public class LevelMusicTrigger : MonoBehaviour
{
    [SerializeField] private MusicTrack trackToPlay;

    private void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayTrack(trackToPlay);
        }
        else
        {
            Debug.LogWarning("[LevelMusicTrigger] No MusicManager instance found.");
        }
    }
}
