using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Burnable : MonoBehaviour
{

    //[Header("Fire Effects")]
    //[SerializeField] private List<FireController> fireEffects;
    //public List<FireController> FireEffects => fireEffects;

    //[Header("Settings Asset")]
    //public BurnableSettings settings;

    //[Header("Instance Settings")]
    //[Tooltip("Multiplier applied to burn speed and spread, editable per object.")]
    //public float burnMultiplier = 1f;

    ////[HideInInspector] public float burnProgress = 0f;
    //private bool isOnFire = false;

    //public float burnProgress { get; private set; }

    //public bool IsOnFire => isOnFire;
    //public bool CanCatchFire => settings != null && settings.canCatchFire;

    //public GameObject FlamableGameObject => gameObject;

    //bool IFlammable.IsOnFire { get => IsOnFire; set => throw new System.NotImplementedException(); }

    //public void Ignite()
    //{
    //    FireSystem.IgniteObject(this);
    //    burnProgress = 1f;
    //    isOnFire = true;
    //}

    //public void Extinguish()
    //{
    //    burnProgress = 0f;
    //    FireSystem.ExtinguishObject(this);
    //}
    //private void SpreadFire()
    //{
    //    List<IFlammable> flammableObjects = GetFlammableObjectsInRange();
    //    foreach (IFlammable flammable in flammableObjects)
    //    {
    //        if (!flammable.IsOnFire)
    //        {
    //            float appliedSpread = settings.spreadAmount * settings.spreadMultiplier * burnMultiplier * Time.deltaTime;
    //        }
    //    }
    //}

    //private void Update()
    //{
    //    if (isOnFire)
    //    {
    //        if (settings.allowSpread)
    //        {
    //            SpreadFire();
    //        }
    //    }
    //    else
    //    {
    //        if (burnProgress >= settings.burnThreshold)
    //        {
    //            isOnFire = true;
    //            Ignite();
    //        }
    //    }
    //}


    
    //private void OnDrawGizmosSelected()
    //{
    //    if (settings != null)
    //    {
    //        Gizmos.color = new Color(1f, 0.3f, 0f, 0.35f);
    //        Gizmos.DrawWireSphere(transform.position, settings.spreadRadius);
    //    }
    //}

    //public void ModifyBurnProgress(float amount)
    //{
    //    burnProgress += amount;
    //    Mathf.Clamp(burnProgress, 0f, 1f);

    //    if (burnProgress == 1f && !isOnFire)
    //    {
    //        Ignite();
    //    }
    //}

    //private List<IFlammable> GetFlammableObjectsInRange()
    //{
    //    Collider[] hits = Physics.OverlapSphere(transform.position, settings.spreadRadius);
    //    List<IFlammable> flammable = new List<IFlammable>();

    //    foreach (var hit in hits)
    //    {
    //        if (hit.TryGetComponent(out IFlammable flammableComponent))
    //        {
    //            flammable.Add(flammableComponent);
    //        }
    //    }

    //    return flammable;
    //}

    //public void Explode()
    //{
    //    throw new System.NotImplementedException();
    //}
}
