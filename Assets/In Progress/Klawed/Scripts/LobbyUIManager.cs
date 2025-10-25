using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerUICanvas;

    

    public void EnablePlayerCanvas(int index)
    {
        if (index == 0)
        {
            Debug.Log("cannot enable player canvas for index 0");
            return;
        }
        playerUICanvas[index].SetActive(true);
    }

    public void DisablePlayerCanvas(int index)
    {
        if (index == 0)
        {
            Debug.Log("cannot disable player canvas for index 0");
            return;
        }
        playerUICanvas[index].SetActive(false);
    }

    

    
}
