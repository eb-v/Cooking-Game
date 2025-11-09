// Skips first audio and plays on every hover afterwards
using UnityEngine;

public class ArmSelectSFXAfterFrame : MonoBehaviour
{
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip hoverClip;
    bool armed;

    void OnEnable() { StartCoroutine(Arm()); }
    System.Collections.IEnumerator Arm() { yield return null; armed = true; }

    public void PlayHoverIfArmed() {
        if (armed && sfx && hoverClip) sfx.PlayOneShot(hoverClip);
    }
}
