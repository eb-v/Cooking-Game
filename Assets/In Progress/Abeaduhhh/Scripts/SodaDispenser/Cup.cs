using UnityEngine;

[RequireComponent(typeof(Grabable))]
public class Cup : MonoBehaviour
{
    public bool isFilled = false;
    public string drinkType = "";

    [SerializeField] public GameObject LidObject;
    public MenuItem drinkItem;


    public void FillCup(string drinkName, Color drinkColor) {
        if (isFilled) return;

        drinkType = drinkName;
        isFilled = true;

        if (LidObject != null) {
            LidObject.SetActive(true);

            Renderer rend = LidObject.GetComponent<Renderer>();
            if (rend != null) {
                rend.material.color = drinkColor;
            }
        }

        Debug.Log("Cup filled with: " + drinkName);
    }

    private GameObject GetRootCupObject() {
        Cup cupComponent = GetComponent<Cup>()
                          ?? GetComponentInChildren<Cup>()
                          ?? GetComponentInParent<Cup>();

        return cupComponent?.gameObject;
    }

    
}
