using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    [Header("Light Material")]
    public Material beaconLightMaterial;
    
    [Header("Emission Settings")]
    [Range(0f, 10f)]
    public float emissionIntensity = 2f;
    
    [Header("Light Colors")]
    public Color redColor = Color.red;
    public Color greenColor = Color.green;
    
    private bool isTrainActive = false;
    
    void Start()
    {
        UpdateBeacon();
    }
    
    void UpdateBeacon()
    {
        if (beaconLightMaterial != null)
        {
            beaconLightMaterial.EnableKeyword("_EMISSION");
            
            // Red when train is active, green otherwise
            Color colorToUse = isTrainActive ? redColor : greenColor;
            beaconLightMaterial.SetColor("_EmissionColor", colorToUse * emissionIntensity);
        }
    }
    
    // Call this method to set whether train is active
    public void SetTrainActive(bool active)
    {
        isTrainActive = active;
        UpdateBeacon();
    }
}