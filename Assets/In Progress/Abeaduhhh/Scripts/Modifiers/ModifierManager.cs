using System.Collections.Generic;
using UnityEngine;

public class ModifierManager : MonoBehaviour {
    [Header("Gameplay Systems")]
    [SerializeField] private BombDropper bombSystem;
    [SerializeField] private OilHazard oilSystem;
    [SerializeField] private RocketBoost jetpackSystem;

    private void Awake() {
        GenericEvent<OnModifiersChoosenEvent>
            .GetEvent("OnModifiersChoosenEvent")
            .AddListener(OnModifiersChosen);
    }

    private void OnModifiersChosen(List<LevelModifiers> mods) {
        foreach (var mod in mods) {
            ApplyModifier(mod);
        }
    }

    private void ApplyModifier(LevelModifiers mod) {
        switch (mod) {
            case LevelModifiers.LandMines:
                bombSystem.StartDropping();
                break;

            //case LevelModifiers.OilSpill:
            //    oilSystem.EnableOilSpills();
            //    break;

            //case LevelModifiers.Jetpack:
            //    jetpackSystem.EnableJetpacks();
            //    break;

            //case LevelModifiers.LowGravity:
            //    Physics.gravity *= 0.5f;
            //    break;

            //case LevelModifiers.ReverseControls:
            //    PlayerController.Instance.EnableReverseControls();
            //    break;

            //case LevelModifiers.SpeedBoost:
            //    PlayerController.Instance.ModifySpeed(1.5f);
            //    break;

                // Add more here
        }
    }
}
