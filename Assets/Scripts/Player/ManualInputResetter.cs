using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ManualInputResetter : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //private void Awake()
    //{
    //    playerInput = GetComponent<PlayerInput>();
    //}

    private void OnEnable()
    {
        playerInput.ActivateInput();
        playerInput.actions.Enable(); 
    }
}
