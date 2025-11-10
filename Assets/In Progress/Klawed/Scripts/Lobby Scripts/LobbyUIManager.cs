using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerUICanvas;

    [SerializeField] private GameObject playerSelectBanner;
    [SerializeField] private GameObject mapSelectionUIObject;

    [SerializeField] private string assignedChannel = "DefaultChannel";

    private void Awake()
    {
        GenericEvent<OnAllPlayersReadyEvent>.GetEvent(assignedChannel).AddListener(OnAllPlayersReady);
    }


    public void EnablePlayerCanvas(int index)
    {
        if (index == 0)
        {
            Debug.Log("cannot enable player canvas for index 0");
            return;
        }


        //GameObject playerUIObj = playerUICanvas[index];
        //LobbyUIEventHandler uiHandler = playerUIObj.GetComponent<LobbyUIEventHandler>();
        //foreach (Transform child in playerUIObj.transform)
        //{
        //    child.gameObject.SetActive(true);
        //}
        playerUICanvas[index].SetActive(true);
    }

    public void DisablePlayerCanvas(int index)
    {
        if (index == 0)
        {
            Debug.Log("cannot disable player canvas for index 0");
            return;
        }
        //GameObject playerUIObj = playerUICanvas[index];
        //LobbyUIEventHandler uiHandler = playerUIObj.GetComponent<LobbyUIEventHandler>();
        //foreach (Transform child in playerUIObj.transform)
        //{
        //    child.gameObject.SetActive(false);
        //}
        if (playerUICanvas[index] != null)
        {
            playerUICanvas[index].SetActive(false);
        }
    }


    public void DisableAllPlayerCanvas()
    {
        for (int i = 1; i < playerUICanvas.Length; i++)
        {
            DisablePlayerCanvas(i);
        }
    }

    private void OnAllPlayersReady()
    {
        DisableAllPlayerCanvas();

        playerSelectBanner.SetActive(false);
        mapSelectionUIObject.SetActive(true);
    }





}
