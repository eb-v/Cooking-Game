using System.Collections.Generic;
using UnityEngine;

public class ModifierManager : MonoBehaviour {
    [Header("Gameplay Systems")]
    [SerializeField] private BombDropper bombSystem;
    [SerializeField] private OilSpawnManager oilSystem;
    [SerializeField] private RocketBoost jetpackSystem;
    [SerializeField] private LightningModifier lightningSystem;
    [SerializeField] private EarthquakeModifier earthquakeSystem;
    [SerializeField] private RobberModifier robberSystem;





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

            case LevelModifiers.OilSpill:
                oilSystem.StartSpawning();
                break;

            case LevelModifiers.Lightning:
                lightningSystem.SpawnLightning();
                break;

            case LevelModifiers.Earthquake:
                earthquakeSystem.StartEarthquake();
                break;

            case LevelModifiers.Robber:
                robberSystem.SpawnRobber();
                break;

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
