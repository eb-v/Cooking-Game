using UnityEngine;

public class AnimManager : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private GameObject playerRagdoll;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        GenericEvent<OnWalkStatusChange>.GetEvent(playerRagdoll.GetInstanceID()).AddListener(ChangeWalkStatus);
    }

    private void ChangeWalkStatus(bool value)
    {
        _animator.SetBool("isWalking", value);
    }
}
