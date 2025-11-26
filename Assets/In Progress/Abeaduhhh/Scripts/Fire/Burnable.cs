using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private List<FireController> fireEffects;
    [SerializeField] private float spreadRadius = 2f;
    //[SerializeField] private float fireImmuneDuration = 5f;


    [Header("Debugging")]
    [ReadOnly]
    [SerializeField] private bool isOnFire = false;
    [ReadOnly]
    [SerializeField] private float burnTimer;
    [ReadOnly]
    [SerializeField] private float burnProgress = 0f;
    //public bool isIgnitionImmune = false;
    //[ReadOnly]
    //[SerializeField] bool burnProgressLocked = false;


    public bool IsOnFire => isOnFire;
    //public bool BurnProgressLocked => burnProgressLocked;


    public void Extinguish()
    {
        FireManager.Instance.ExtinguishObject(ref isOnFire, ref burnProgress, fireEffects);
        FireManager.Instance.UnRegisterBurningObject(this);
        
    }

    public void Ignite()
    {
        FireManager.Instance.IgniteObject(ref isOnFire, ref burnProgress, fireEffects);
        FireManager.Instance.RegisterBurningObject(this);
        GenericEvent<OnObjectIgnited>.GetEvent(gameObject.GetInstanceID().ToString()).Invoke();
    }

    public void Update()
    {
        if (isOnFire)
        {
            FireManager.Instance.SpreadFire(transform.position, spreadRadius, this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spreadRadius);
    }

    public void ModifyBurnProgress(float amount)
    {
        burnProgress += amount;
        burnProgress = Mathf.Clamp01(burnProgress);
        if (burnProgress <= 0f && isOnFire)
        {
            Extinguish();
        }
        else if (burnProgress >= 1f && !isOnFire)
        {
            Ignite();
        }
    }

    //public IEnumerator LockBurnProgress(float duration)
    //{
    //    burnProgressLocked = true;
    //    yield return new WaitForSeconds(duration);
    //    burnProgressLocked = false;
    //}
    

}
