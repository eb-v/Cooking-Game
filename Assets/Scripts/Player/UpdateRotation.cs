using Unity.VisualScripting;
using UnityEngine;

public class UpdateRotation : MonoBehaviour
{

    [SerializeField] private GameObject playerRagdoll;
    private Vector3 direction;
    public float rotationSpeed = 5f;


    private void OnEnable()
    {
        GenericEvent<OnMoveInput>.GetEvent(playerRagdoll.GetInstanceID()).AddListener(UpdateDirection);
    }

    private void Update()
    {
        SetRotation();
    }

    private void SetRotation()
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void UpdateDirection(Vector2 direction)
    {
        this.direction = direction;
    }
}
