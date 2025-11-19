using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class SodaDispenser : MonoBehaviour {
    public Cup cupUnderDispenser;

    private void OnTriggerEnter(Collider other) {
        Cup cup = other.GetComponent<Cup>();
        if (cup != null) {
            cupUnderDispenser = cup;
            Debug.Log("Cup detected. Ready to fill.");
        }
    }

    private void OnTriggerExit(Collider other) {
        Cup cup = other.GetComponent<Cup>();
        if (cup == cupUnderDispenser) {
            cupUnderDispenser = null;
        }
    }

    public void Dispense(string drinkName, Color color) {
        if (cupUnderDispenser == null) {
            Debug.Log("No cup under dispenser!");
            return;
        }

        cupUnderDispenser.FillCup(drinkName, color);
    }
}
