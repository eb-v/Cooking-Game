using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    [Header("Light Material")]
    public Material beaconLightMaterial;
    
    [Header("Emission Settings")]
    [Range(0f, 10f)]
    public float emissionIntensity = 2f;
    
    [Header("Control Settings")]
    public bool enableBeacon = true; // Set to false to turn off beacon
    
    // ========== CYCLING SETTINGS (Comment out if not using cycle) ==========
    [Header("Beacon Cycle Settings")]
    public bool useCycle = true; // Set to false to manually control color
    public float cycleDuration = 3f; // Time for each color in seconds
    
    private Color[] beaconColors = new Color[] { Color.red, new Color(1f, 0.5f, 0f), Color.green }; // Red, Orange, Green
    private int currentColorIndex = 0;
    private float timer = 0f;
    // ========== END CYCLING SETTINGS ==========
    
    [Header("Manual Color Control (when useCycle is false)")]
    public Color manualColor = Color.red; // Color to use when not cycling
    
    void Start()
    {
        UpdateBeacon();
    }
    
    void Update()
    {
        // ========== CYCLING CODE (Comment out if not using cycle) ==========
        if (useCycle && enableBeacon)
        {
            // Cycle the beacon light color
            timer += Time.deltaTime;
            
            if (timer >= cycleDuration)
            {
                timer = 0f;
                currentColorIndex = (currentColorIndex + 1) % beaconColors.Length;
                UpdateBeacon();
            }
        }
        // ========== END CYCLING CODE ==========
        else
        {
            UpdateBeacon();
        }
    }
    
    void UpdateBeacon()
    {
        if (beaconLightMaterial != null)
        {
            if (enableBeacon)
            {
                // Turn on the beacon
                beaconLightMaterial.EnableKeyword("_EMISSION");
                
                Color colorToUse = useCycle ? beaconColors[currentColorIndex] : manualColor;
                beaconLightMaterial.SetColor("_EmissionColor", colorToUse * emissionIntensity);
            }
            else
            {
                // Turn off the beacon
                beaconLightMaterial.DisableKeyword("_EMISSION");
                beaconLightMaterial.SetColor("_EmissionColor", Color.black);
            }
        }
    }
    
    // Public method to manually set the beacon state
    public void SetBeaconState(bool state)
    {
        enableBeacon = state;
        UpdateBeacon();
    }
    
    // Public method to set a specific color (when not cycling)
    public void SetBeaconColor(Color color)
    {
        manualColor = color;
        if (!useCycle)
        {
            UpdateBeacon();
        }
    }
}