using UnityEngine;

public class EarthquakeController : MonoBehaviour {
    public EarthquakeCameraShake cameraShake;
    public EarthquakeKnockOff quakePhysics;

    public float shakeDuration = 0.7f;
    public float shakeMagnitude = 0.15f;

    public void ActivateEarthquake() {
        cameraShake.Shake(shakeDuration, shakeMagnitude);
        quakePhysics.TriggerEarthquake();
    }
}
