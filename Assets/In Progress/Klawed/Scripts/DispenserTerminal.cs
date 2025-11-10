using UnityEngine;

public class DispenserTerminal : MonoBehaviour
{
    [SerializeField] private string _assignedChannel;

    // player touches button on terminal with hand
    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "LeftHand" && other.name != "RightHand") return;
        GenericEvent<DispenserButtonPressedEvent>.GetEvent(_assignedChannel).Invoke();
    }

}
