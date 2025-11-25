using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CaptureRenderTexture : MonoBehaviour
{
    [SerializeField] private Camera renderCamera;
    [SerializeField] private RawImage imageDisplay;

    public void CaptureAndDisplay(string fileName = "CameraSnapshot.png")
    {
        RenderTexture rt = renderCamera.targetTexture;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D snapshot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        snapshot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        snapshot.Apply();

        RenderTexture.active = currentRT;

        if (imageDisplay != null)
            imageDisplay.texture = snapshot;

        byte[] bytes = snapshot.EncodeToPNG();

        // Define the folder path inside Assets
        string folderPath = Path.Combine(Application.dataPath, "Images/Food/Ingredients");

        // Create folder if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Created folder: " + folderPath);
        }

        // Full file path
        string filePath = Path.Combine(folderPath, fileName);

        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Saved snapshot to: " + filePath);

#if UNITY_EDITOR
        // Refresh the editor so the new file appears immediately
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
