using UnityEngine;

public class CrateScript : MonoBehaviour
{
    private void Start()
    {
        GenericEvent<ReleaseCrate>.GetEvent("releaseCrate").AddListener(OnReleaseCrate);
    }

    private void OnReleaseCrate()
    {
        Destroy(this.GetComponent<FixedJoint>());
    }

}
