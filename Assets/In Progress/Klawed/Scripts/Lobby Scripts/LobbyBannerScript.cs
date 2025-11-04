using UnityEngine;

public class LobbyBannerScript : MonoBehaviour
{
    [SerializeField] private string assignedChannel = "DefaultChannel";
    [SerializeField] private GameObject playerSelectBanner;
    [SerializeField] private GameObject mapSelectBanner;
    
    private void Awake()
    {
        GenericEvent<OnAllPlayersReadyEvent>.GetEvent(assignedChannel).AddListener(OnAllPlayersReady);
    }

    private void OnAllPlayersReady()
    {
        playerSelectBanner.SetActive(false);
        mapSelectBanner.SetActive(true);
    }


}
