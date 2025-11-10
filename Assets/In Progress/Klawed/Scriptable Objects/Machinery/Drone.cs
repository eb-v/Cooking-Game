using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Drone", menuName = "Objects/Drone")]
public class Drone : ScriptableObject
{
    [Header("Drone Settings")]
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float explosionChance;

    public float ExplosionChance => explosionChance;
    public float HorizontalSpeed => horizontalSpeed;
    public float VerticalSpeed => verticalSpeed;
}
