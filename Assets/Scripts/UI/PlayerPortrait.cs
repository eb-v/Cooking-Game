using UnityEngine;
using UnityEngine.UI;

public class PlayerPortrait : MonoBehaviour
{
    public Camera portraitCamera;
    private RenderTexture portraitTexture;
    public RawImage[] hudSlots;
    public RawImage hudImage;

    private void Start()
    {
        portraitTexture = new RenderTexture(256, 256, 16);
        portraitTexture.name = gameObject.name + "_Portrait";

        portraitCamera.targetTexture = portraitTexture;

        

    }
}
