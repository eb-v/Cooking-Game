//using UnityEngine;

//public class FireEffect : MonoBehaviour {
//    [SerializeField] private ParticleSystem fireParticleSystem;
//    [SerializeField] private AudioSource fireAudioSource;

//    private void Awake() {
//        if (fireParticleSystem == null) {
//            Debug.LogError("FireEffect: Fire Particle System is not assigned.");
//        } else {
//            fireParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
//        }

//        if (fireAudioSource == null) {
//            Debug.LogWarning("FireEffect: Fire Audio Source is not assigned.");
//        } else {
//            fireAudioSource.Stop();
//        }
//    }

//    public void StartFire() {
//        if (fireParticleSystem != null && !fireParticleSystem.isPlaying) {
//            fireParticleSystem.Play();
//        }
//        if (fireAudioSource != null && !fireAudioSource.isPlaying) {
//            fireAudioSource.Play();
//        }
//    }

//    public void StopFire() {
//        if (fireParticleSystem != null && fireParticleSystem.isPlaying) {
//            fireParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
//        }
//        if (fireAudioSource != null && fireAudioSource.isPlaying) {
//            fireAudioSource.Stop();
//        }
//    }
//}
