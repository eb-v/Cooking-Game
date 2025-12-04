using UnityEngine;

// All limbs that have this script can take damage and be disconnected when their HP reaches 0
public class LimbHP : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;
    public RagdollController rc;
    private RagdollJoint ragdollJoint;

    public bool canBePoppedByProximity = true;
    public bool IsBroken => currentHP <= 0f;

    private void Awake()
    {
        ragdollJoint = GetComponent<RagdollJoint>();
        GenericEvent<PlayerDestroy>.GetEvent(transform.root.gameObject.GetInstanceID().ToString()).AddListener(DestoryLimb);
    }

    private void Start()
    {
        string playerName = gameObject.transform.root.name;
        GenericEvent<LimbDamaged>.GetEvent(playerName + gameObject.name).AddListener(ApplyDamage);
        currentHP = maxHP;
    }

    public void ApplyDamage(float damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP <= 0)
        {
            // reset values
            Reset();

            // Limb is disconnected
            GenericEvent<OnJointRemoved>.GetEvent(gameObject.transform.root.GetInstanceID().ToString()).Invoke();
            rc.DisconnectJoint(ragdollJoint.GetJointName());
        }
    }

    private void Reset()
    {
        currentHP = maxHP;
    }

    [ContextMenu("Inflict Max Damage")]
    public void InflictMaxDamage()
    {
        ApplyDamage(maxHP);
    }

    public void DisconnectLimb()
    {
        GenericEvent<OnJointRemoved>.GetEvent(gameObject.transform.root.GetInstanceID().ToString()).Invoke();
        rc.DisconnectJoint(ragdollJoint.GetJointName());
    }
    
    public void DestoryLimb()
    {
        Destroy(gameObject);
    }
}
