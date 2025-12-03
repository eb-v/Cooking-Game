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

        Debug.Log("ModifierManager called!");

        GenericEvent<OnModifiersChoosenEvent>
            .GetEvent("OnModifiersChoosenEvent")
            .AddListener(OnModifiersChosen);
    }


    private void OnModifiersChosen(List<LevelModifiers> mods) {
        Debug.Log("Received modifiers: " + string.Join(", ", mods));
        foreach (var mod in mods) ApplyModifier(mod);
    }

    private void ApplyModifier(LevelModifiers mod) {
        switch (mod) {
            case LevelModifiers.LandMines:
                Debug.Log("Starting Landmine Modifier");
                bombSystem.StartDropping();
                break;

            case LevelModifiers.OilSpill:
                Debug.Log("Starting OilSpill Modifier");
                oilSystem.StartSpawning();
                break;

            case LevelModifiers.Lightning:
                Debug.Log("Starting Lightning Modifier");
                lightningSystem.TriggerLightning();
                break;

            case LevelModifiers.Earthquake:
                Debug.Log("Starting Earthquake Modifier");
                earthquakeSystem.EnableModifier();
                break;

            case LevelModifiers.Robber:
                Debug.Log("Starting Robber Modifier");
                robberSystem.SpawnRobber();
                break;

            case LevelModifiers.Jetpack:
                Debug.Log("Starting Jetpack Modifier");
                jetpackSystem.PerformRocketBoost();
                break;

            case LevelModifiers.CloseProximity:
                Debug.Log("Starting CloseProximity Modifier");
                closeProximitySystem.ActivateModifier();
                break;
        }
    }
}
