using UnityEngine;

[CreateAssetMenu(fileName = "GrabDatabase", menuName = "Database/GrabDatabase")]
public class GrabDatabase : ScriptableObject
{
    private static UDictionary<IGrabable, GrabData> GrabMap = new UDictionary<IGrabable, GrabData>();

    public static GrabData GetGrabData(IGrabable grabable)
    {
        if (GrabMap.ContainsKey(grabable))
        {
            return GrabMap[grabable];
        }
        else
        {
            Debug.LogWarning($"GrabDatabase: No GrabData found for {grabable}. Returning null.");
            return null;
        }
    }
}
