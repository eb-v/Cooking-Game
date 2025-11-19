using UnityEngine;

public class Cup : MonoBehaviour {
    public bool isFilled = false;
    public string drinkType = "";
    public Renderer liquidRenderer;

    public void FillCup(string drinkName, Color drinkColor) {
        if (isFilled) return;

        drinkType = drinkName;
        isFilled = true;

        if (liquidRenderer != null) {
            liquidRenderer.material.color = drinkColor;
            liquidRenderer.enabled = true;
        }

        Debug.Log("Cup filled with " + drinkName);
    }
}
