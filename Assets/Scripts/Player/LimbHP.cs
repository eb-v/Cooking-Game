using UnityEngine;

// All limbs that have this script can take damage and be disconnected when their HP reaches 0
public class LimbHP : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;


    private void Start()
    {
        string playerName = gameObject.transform.root.name;
        GenericEvent<LimbDamaged>.GetEvent(playerName + gameObject.name).AddListener(ApplyDamage);
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
