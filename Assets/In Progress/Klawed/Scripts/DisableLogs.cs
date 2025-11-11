using UnityEngine;

public class DisableLogs : MonoBehaviour
{
    private void Awake()
    {
        Debug.unityLogger.logEnabled = false;
    }
}
