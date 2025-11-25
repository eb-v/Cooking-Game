using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerUICanvas;

    [SerializeField] private GameObject playerSelectBanner;
    [SerializeField] private GameObject mapSelectionUIObject;

    [SerializeField] private string assignedChannel = "DefaultChannel";
    [ReadOnly]
    [SerializeField] private int currentCanvasIndex = 1;

    private void Awake()
    {
        GenericEvent<OnAllPlayersReadyEvent>.GetEvent(assignedChannel).AddListener(OnAllPlayersReady);
        GenericEvent<OnPlayerJoinedEvent>.GetEvent("PlayerJoined").AddListener(OnPlayerJoined);
    }

    private void OnPlayerJoined(GameObject player)
    {
        EnablePlayerCanvas();
    }
    private void EnablePlayerCanvas()
    {
        playerUICanvas[currentCanvasIndex].SetActive(true);
        currentCanvasIndex++;
    }
    public void DisablePlayerCanvas(int index)
    {
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
