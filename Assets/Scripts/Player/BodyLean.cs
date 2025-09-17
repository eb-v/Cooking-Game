using UnityEngine;

public class BodyLean : MonoBehaviour
{
    private ConfigurableJoint _bodyJoint;
    private bool _leanForward;
    private bool _leanBackward;
    [SerializeField] private float _angleMultiplier = 5f;
    private Quaternion _initialRotation;
    private float _leanAngleX = 0f;

    private void Awake()
    {
        GenericEvent<OnLeanForwardInput>.GetEvent(transform.root.gameObject.GetInstanceID()).AddListener(() => { _leanForward = true; });
        GenericEvent<OnLeanForwardCancel>.GetEvent(transform.root.gameObject.GetInstanceID()).AddListener(() => { _leanForward = false; });
        GenericEvent<OnLeanBackwardInput>.GetEvent(transform.root.gameObject.GetInstanceID()).AddListener(() => { _leanBackward = true; });
        GenericEvent<OnLeanBackwardCancel>.GetEvent(transform.root.gameObject.GetInstanceID()).AddListener(() => { _leanBackward = false; });

        _bodyJoint = GetComponent<ConfigurableJoint>();
        _initialRotation = _bodyJoint.targetRotation;
    }

    private void Update()
    {
        if (_leanForward && !_leanBackward)
        {
            PerformLeanForward();
        }

        if (_leanBackward && !_leanForward)
        {
            PerformLeanBackward();
        }

        //if (!_leanForward && !_leanBackward)
        //{
        //    PerformReturnToNeutral();
        //}
    }

    private void PerformLeanForward()
    {
        Vector3 axis = Vector3.right;
        _leanAngleX = Mathf.Clamp(_leanAngleX - _angleMultiplier * Time.deltaTime, -80f, 80f);

        // Quaternion deltaRotation = Quaternion.AngleAxis(_leanAngleX, axis);

        Quaternion newTargetRotation = Quaternion.Euler(_leanAngleX, 0f, 0f);
        _bodyJoint.targetRotation = newTargetRotation;
    }

    private void PerformLeanBackward()
    {
        Vector3 axis = Vector3.right;
        _leanAngleX = Mathf.Clamp(_leanAngleX + _angleMultiplier * Time.deltaTime, -80f, 80f);

        //Quaternion deltaRotation = Quaternion.AngleAxis(_leanAngleX, axis);

        Quaternion newTargetRotation = Quaternion.Euler(_leanAngleX, 0f, 0f);
        _bodyJoint.targetRotation = newTargetRotation;
    }

    private void PerformReturnToNeutral()
    {
        _bodyJoint.targetRotation = Quaternion.Slerp(_bodyJoint.targetRotation, _initialRotation, 5f * Time.deltaTime);
    }


}
