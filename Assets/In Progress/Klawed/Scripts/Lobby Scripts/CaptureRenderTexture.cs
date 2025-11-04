using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CaptureRenderTexture : MonoBehaviour
{
    [SerializeField] private Camera renderCamera;
    [SerializeField] private RawImage imageDisplay; // optional - UI image to show captured frame

    public void CaptureAndDisplay()
    {
        RenderTexture rt = renderCamera.targetTexture;

        // Make sure the camera rendered
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        // Create a new Texture2D with same dimensions as RenderTexture
        Texture2D snapshot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        snapshot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        snapshot.Apply();

        RenderTexture.active = currentRT;

        // Display snapshot on UI (if provided)
        if (imageDisplay != null)
            imageDisplay.texture = snapshot;

        // Optionally save to file
        byte[] bytes = snapshot.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/CameraSnapshot.png", bytes);
        Debug.Log("Saved snapshot to " + Application.dataPath + "/CameraSnapshot.png");
    }
}
