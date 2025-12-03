using UnityEngine;

public class Player_Knife : MonoBehaviour {

    [SerializeField] public GameObject knife;
    private void OnEnable() {
        knife.SetActive(false);
    }

    public void ShowKnife() {
        knife.SetActive(true);
    }
    public void HideKnife() {
        knife.SetActive(false);
    }
}
