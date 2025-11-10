using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    private void Awake()
    {
        GenericEvent<OnGameStartEvent>.GetEvent("GameStart").Invoke();
    }
}
