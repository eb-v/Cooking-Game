using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;     
    public bool countDown = true;                

    void Reset() {
        if (fillImage == null)
            fillImage = transform.Find("Fill")?.GetComponent<Image>();
    }

    void Update()
    {
        var gm = KitchenGameManager.Instance;
        if (gm == null || fillImage == null) return;

        float dur  = Mathf.Max(0.01f, gm.GetGamePlayingDuration());
        float left = Mathf.Clamp(gm.GetGamePlayingTimeLeft(), 0f, dur);
        float norm = countDown ? (left / dur) : 1f - (left / dur);

        fillImage.fillAmount = norm;           
    }
}
