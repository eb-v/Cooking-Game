using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Burnable : MonoBehaviour, IFlammable
{
    [Header("Fire Related Values")]
    [SerializeField] private List<FireController> fireEffects;
    [SerializeField] public BurnableSettings settings;
    [SerializeField] private float spreadRadius = 2f;
    [field: SerializeField] public bool IsOnFire { get; set; }
    [ReadOnly]
    [SerializeField] private float burnProgress = 0f;
    public float BurnProgress => burnProgress;
    public GameObject FlamableGameObject => gameObject;
    public List<FireController> FireEffects => fireEffects;


    public void Extinguish()
    {
        FireSystem.ExtinguishObject(this);
        burnProgress = 0f;
    }

    public void Ignite()
    {
        burnProgress = 1f;
        FireSystem.IgniteObject(this);
        GenericEvent<OnObjectIgnited>.GetEvent(gameObject.GetInstanceID().ToString()).Invoke();
    }

    public void ModifyBurnProgress(float amount)
    {
        burnProgress += amount;
        Mathf.Clamp01(burnProgress);
        if (burnProgress >= 1f && !IsOnFire)
        {
            Ignite();
        }
        else if (burnProgress <= 0f && IsOnFire)
        {
            Extinguish();
        }
    }

    public virtual void Update()
    {
        if (IsOnFire)
        {
            OnFireLogic();

            return;
        }
    }

    public virtual void FixedUpdate()
    {
        if (IsOnFire)
        {

            return;
        }
    }

    public virtual void OnFireLogic()
    {
        if (settings.allowSpread)
        {
            if (FireSystem.Instance == null)
            {
                Debug.LogError("FireSystem instance is null!");
            }
            FireSystem.Instance.SpreadFire(gameObject.transform.position, spreadRadius);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spreadRadius);
    }

}
