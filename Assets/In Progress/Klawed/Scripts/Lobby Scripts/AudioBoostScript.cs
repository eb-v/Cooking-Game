using UnityEngine;
using UnityEngine.Audio;

public class AudioBoostScript : MonoBehaviour
{
    public AudioMixer mixer;
    public string exposedParam = "SlotMachineVolume";
    public float boostAmount = 10f;

    public void Awake()
    {
        SetVolume();
    }


    public void SetVolume()
    {
        mixer.SetFloat(exposedParam, boostAmount);
    }
}
