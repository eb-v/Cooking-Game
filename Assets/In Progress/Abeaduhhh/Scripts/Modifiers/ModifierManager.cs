using System.Collections.Generic;
using UnityEngine;

public class ModifierManager : MonoBehaviour {
    [Header("Gameplay Systems")]
    [SerializeField] private BombDropper bombSystem;
    [SerializeField] private OilSpawnManager oilSystem;
    [SerializeField] private LightningModifier lightningSystem;
    [SerializeField] private EarthquakeModifier earthquakeSystem;
    [SerializeField] private RobberModifier robberSystem;
    [SerializeField] private RocketBoost jetpackSystem;
    [SerializeField] private CloseProximityManager closeProximitySystem;







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
                lightningSystem.TriggerLightning();
                break;

            case LevelModifiers.Earthquake:
                earthquakeSystem.StartEarthquake();
                break;

            case LevelModifiers.Robber:
                robberSystem.SpawnRobber();
                break;

            case LevelModifiers.Jetpack:
                jetpackSystem.PerformRocketBoost();
                break;

            case LevelModifiers.CloseProximity:
                closeProximitySystem.ActivateModifier();
                break;
        }
    }
}
