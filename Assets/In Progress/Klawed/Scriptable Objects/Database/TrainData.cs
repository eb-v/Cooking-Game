using UnityEngine;

[CreateAssetMenu(fileName = "TrainData", menuName = "Scriptable Objects/Data/TrainData")]
public class TrainData : ScriptableObject
{
    public float delayBeforeLaunch = 2.0f;
    public float trainSpeed = 100.0f;
    public float stopPositionX = 0.0f;
    public float resetDelay = 10.0f; // Time before train resets after stopping
    public float randomCallInterval = 20.0f; // Random call every 20 seconds
    public float knockbackForce = 1000f; // Force applied when hitting objects
    public float upwardForce = 0.5f; // Upward component for knockback (0-1)


}
