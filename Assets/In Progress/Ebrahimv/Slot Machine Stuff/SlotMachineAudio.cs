using System.Collections;
using UnityEngine;

public class SlotMachineAudio : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip leverAudio;
    [SerializeField] private AudioClip spinAudio;
    [SerializeField] private AudioClip thumpAudio;
    [SerializeField] private AudioClip winAudio;

    [Header("References")]
    [SerializeField] private SlotMachineScript slotMachine;

    [Header("Timing")]
    [SerializeField] private float spinDuration1 = 2f;
    [SerializeField] private float spinDuration2 = 3f;
    [SerializeField] private float spinDuration3 = 4f;

    private int stoppedSlotsCount = 0;

    // Call this method when the spin button is clicked
    public void OnSpinButtonClicked()
    {
        StartCoroutine(PlaySpinSequence());
    }

    private IEnumerator PlaySpinSequence()
    {
        // Reset the stopped slots counter
        stoppedSlotsCount = 0;

        // Play lever audio and spin audio simultaneously
        if (audioSource != null && leverAudio != null)
        {
            audioSource.PlayOneShot(leverAudio);
        }

        if (audioSource != null && spinAudio != null)
        {
            audioSource.PlayOneShot(spinAudio);
        }

        // Start the slot machine spinning
        if (slotMachine != null)
        {
            slotMachine.StartSpinningAll();
        }

        // Start coroutines to play thump sounds when each slot stops
        StartCoroutine(PlayThumpAtSlotStop(spinDuration1));
        StartCoroutine(PlayThumpAtSlotStop(spinDuration2));
        StartCoroutine(PlayThumpAtSlotStop(spinDuration3));

        yield return null;
    }

    private IEnumerator PlayThumpAtSlotStop(float duration)
    {
        // Wait for the slot to stop spinning
        yield return new WaitForSeconds(duration);

        // Play thump sound
        if (audioSource != null && thumpAudio != null)
        {
            audioSource.PlayOneShot(thumpAudio);
        }

        // Increment stopped slots counter
        stoppedSlotsCount++;

        // If all three slots have stopped, play the win audio
        if (stoppedSlotsCount >= 3)
        {
            yield return new WaitForSeconds(0.2f); // Small delay after the last thump
            if (audioSource != null && winAudio != null)
            {
                audioSource.PlayOneShot(winAudio);
            }
        }
    }
}
