using UnityEngine;

// All limbs that have this script can take damage and be disconnected when their HP reaches 0
public class LimbHP : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;


    private void Awake()
    {
        GenericEvent<LimbDamaged>.GetEvent(gameObject.name).AddListener(ApplyDamage);
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    private void ApplyDamage(float damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP <= 0)
        {
            // reset values
            Reset();

            // Limb is disconnected
            RagdollJoint ragdollJoint = gameObject.GetComponent<RagdollJoint>();
            GenericEvent<OnRemoveJoint>.GetEvent(gameObject.transform.root.name).Invoke(ragdollJoint.GetJointName());
        }
    }

    private void Reset()
    {
        currentHP = maxHP;
    }


}
