using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MapSelectionScript : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private int maxIndex = 1;
    [SerializeField] private int minIndex = -1;
    [SerializeField] private SpringAPI springAPI;
    [SerializeField] private int currentIndex = 0;
    [SerializeField] private string assignedPlayerName = "Player 1";

    private void Awake()
    {
        GenericEvent<OnNextOptionInput>.GetEvent(assignedPlayerName).AddListener(OnNextOption);
        GenericEvent<OnPreviousOptionInput>.GetEvent(assignedPlayerName).AddListener(OnPreviousOption);
        GenericEvent<OnSelectInput>.GetEvent(assignedPlayerName).AddListener(OnSelect);
    }
    
    private void OnNextOption()
    {
        if (gameObject.activeInHierarchy == false) return;
        IncrementIndex();
    }

    private void OnPreviousOption()
    {
        if (gameObject.activeInHierarchy == false) return;
        DecrementIndex();
    }

    private void OnSelect()
    {
        if (gameObject.activeInHierarchy == false) return;
        SceneManager.LoadScene("Level 2");
    }



    private void IncrementIndex()
    {
        currentIndex++;
        if (currentIndex > maxIndex)
        {
            currentIndex = minIndex;
        }
        springAPI.SetGoalValue(currentIndex);
    }

    private void DecrementIndex()
    {
        currentIndex--;
        if (currentIndex < minIndex)
        {
            currentIndex = maxIndex;
        }
        springAPI.SetGoalValue(currentIndex);
    }


}
