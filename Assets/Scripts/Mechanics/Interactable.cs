using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Collider interactCollider;

    private void Awake()
    {
       interactCollider = transform.Find("InteractCollider").GetComponent<Collider>();
    }

    public void Interact(GameObject player)
    {

    }

    public void AltInteract(GameObject player)
    {

    }

}
