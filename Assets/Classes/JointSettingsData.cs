using UnityEngine;

[System.Serializable]
public struct JointSettingsData
{
    // Connections
    public Transform parent;
    public Rigidbody connectedBody; // Store a reference!
    public Quaternion localRotation;
    public Vector3 anchor;
    public Vector3 connectedAnchor;
    public bool autoConfigureConnectedAnchor;

    // Motion Control
    public ConfigurableJointMotion xMotion;
    public ConfigurableJointMotion yMotion;
    public ConfigurableJointMotion zMotion;
    public ConfigurableJointMotion angularXMotion;
    public ConfigurableJointMotion angularYMotion;
    public ConfigurableJointMotion angularZMotion;

    // Limits (require special handling since SoftJointLimit is NOT serializable)
    public SoftJointLimit linearLimit; // Unity allows setting the struct copy
    public SoftJointLimitSpring linearLimitSpring;
    public SoftJointLimit lowAngularXLimit;
    public SoftJointLimit highAngularXLimit;
    public SoftJointLimit angularYLimit;
    public SoftJointLimit angularZLimit;
    public SoftJointLimitSpring angularXLimitSpring;
    public SoftJointLimitSpring angularYZLimitSpring;

    // Drives (require special handling since JointDrive is NOT serializable)
    public JointDrive xDrive;
    public JointDrive yDrive;
    public JointDrive zDrive;
    public JointDrive angularXDrive;
    public JointDrive angularYZDrive;
    public JointDrive slerpDrive;

    // Other Settings
    public bool configuredInWorldSpace;
    public bool swapBodies;
    public float breakForce;
    public float breakTorque;
    public bool enableCollision;
    public bool enablePreprocessing;
}