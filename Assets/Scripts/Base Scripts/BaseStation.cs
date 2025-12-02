using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BaseStation : MonoBehaviour, IInteractable, IAltInteractable
{
   
    public virtual void OnAltInteract(GameObject player)
    {
    }

    public virtual void OnInteract(GameObject player)
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

}
