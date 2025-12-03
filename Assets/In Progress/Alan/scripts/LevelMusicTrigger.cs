using UnityEngine;
using System.Collections;

public class LevelMusicTrigger : MonoBehaviour
{
    [SerializeField] private MusicTrack trackToPlay;

    private IEnumerator Start()
    {
        // Debug.Log($"[LevelMusicTrigger] Start in scene '{gameObject.scene.name}'.");

        // Wait until MusicManager.Instance is set
        while (MusicManager.Instance == null)
        {
            // Debug.Log("[LevelMusicTrigger] Waiting for MusicManager.Instance...");
            yield return null;
        }

        // Debug.Log($"[LevelMusicTrigger] MusicManager found: '{MusicManager.Instance.gameObject.name}', playing '{trackToPlay}'.");
        MusicManager.Instance.PlayTrack(trackToPlay);
    }
}
