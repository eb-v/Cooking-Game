using UnityEngine;

public class FireUpdateDriver : MonoBehaviour
{
    private void LateUpdate()
    {
        FireSystem.Instance.EndFrame();
    }
}
