using UnityEngine;

public class Player_RollingPin : MonoBehaviour {

    [SerializeField] public GameObject rollingpin;
    private void OnEnable() {
        rollingpin.SetActive(false);
    }

    public void ShowRollingPin() {
        rollingpin.SetActive(true);
    }
    public void HideRollingPin() {
        rollingpin.SetActive(false);
    }
}
