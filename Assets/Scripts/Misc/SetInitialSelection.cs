using UnityEngine;
using UnityEngine.EventSystems;

public class SetInitialSelection : MonoBehaviour
{
    [SerializeField] private GameObject initialSelection;

    void Start()
    {
        EventSystem eventSystem = EventSystem.current;

        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(initialSelection);
        }
        else
        {
            Debug.LogWarning("No EventSystem found in the scene. Cannot set initial selection.");
        }
    }
}
