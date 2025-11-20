using UnityEngine;

public class RawAudioTest : MonoBehaviour
{
    public AudioClip testClip;

    private void Start()
    {
        if (testClip == null)
        {
            Debug.LogWarning("[RawAudioTest] No testClip assigned.");
            return;
        }

        // Create a temporary AudioSource on THIS object
        var src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f;   // 2D
        src.volume = 1f;

        src.PlayOneShot(testClip, 1f);
        Debug.Log("[RawAudioTest] Playing raw test clip: " + testClip.name);
    }
}
