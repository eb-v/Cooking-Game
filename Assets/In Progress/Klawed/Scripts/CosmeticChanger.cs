using UnityEngine;
using UnityEngine.UI;

public class CosmeticChanger : MonoBehaviour
{
    [Header("Body Colors")]
    public Material[] robotColors;
    [Tooltip("Which material slot holds the body color on your renderers?")]
    public int colorMaterialSlot = 1;

    [Header("Faces (Sprites)")]
    public Sprite[] faceSprites;

    [Tooltip("If FaceCanvas/FaceImage is missing, we’ll attach one under a transform whose name contains this (e.g., DEF_Head).")]
    public string headNameContains = "face_";

    [Tooltip("Local position of the face canvas (under the head).")]
    public Vector3 faceCanvasLocalPos = new Vector3(0f, 0.065f, 0.08f);

    [Tooltip("Local euler rotation of the face canvas.")]
    public Vector3 faceCanvasLocalEuler = Vector3.zero;

    [Tooltip("Uniform local scale for the world-space canvas (smaller = smaller face).")]
    public float faceCanvasScale = 0.0012f;

    [Tooltip("Pixel size of the Image rect.")]
    public Vector2 faceImagePixels = new Vector2(256, 128);

    public bool facePreserveAspect = true;

    private int _colorIdx = 0;
    private int _faceIdx  = 0;

    // ====== Public API for UI ======
    public void NextColor()     { _colorIdx = (_colorIdx + 1) % Mathf.Max(1, robotColors?.Length ?? 1); }
    public void PreviousColor() { _colorIdx = (_colorIdx - 1 + Mathf.Max(1, robotColors?.Length ?? 1)) % Mathf.Max(1, robotColors?.Length ?? 1); }
    public void NextFace()      { _faceIdx  = (_faceIdx  + 1) % Mathf.Max(1, faceSprites?.Length ?? 1); }
    public void PreviousFace()  { _faceIdx  = (_faceIdx  - 1 + Mathf.Max(1, faceSprites?.Length ?? 1)) % Mathf.Max(1, faceSprites?.Length ?? 1); }

    public void ApplyColor(GameObject player)
    {
        if (!player || robotColors == null || robotColors.Length == 0) return;

        void Swap(Renderer r)
        {
            if (!r) return;
            var mats = r.materials;
            if (mats == null || mats.Length == 0) return;
            int slot = Mathf.Clamp(colorMaterialSlot, 0, mats.Length - 1);
            mats[slot] = robotColors[_colorIdx];
            r.materials = mats;
        }

        var skins  = player.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        var meshes = player.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < skins.Length;  i++) Swap(skins[i]);
        for (int i = 0; i < meshes.Length; i++) Swap(meshes[i]);
    }

    public void ApplyFace(GameObject player)
    {
        if (!player || faceSprites == null || faceSprites.Length == 0) return;

        var img = EnsureFaceUI(player);
        if (!img) return;

        img.preserveAspect = facePreserveAspect;
        img.sprite = faceSprites[_faceIdx % faceSprites.Length];

        var rt = img.rectTransform;
        rt.sizeDelta = faceImagePixels; // pixels; world-space scale controls final size
    }

    // ====== Helpers ======
    private Image EnsureFaceUI(GameObject player)
    {
        // Find or create FaceCanvas under the head
        Transform head = FindFirstByNameContains(player.transform, headNameContains);
        if (!head) head = player.transform;

        Transform faceCanvas = head.Find("FaceCanvas");
        if (!faceCanvas)
        {
            var go = new GameObject("FaceCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            faceCanvas = go.transform;
            faceCanvas.SetParent(head, false);

            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 50;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 100;

            var rt = (RectTransform)faceCanvas;
            rt.localPosition    = faceCanvasLocalPos;
            rt.localEulerAngles = faceCanvasLocalEuler;
            rt.localScale       = Vector3.one * faceCanvasScale;
        }

        // Find or create FaceImage child
        Transform faceImage = faceCanvas.Find("FaceImage");
        if (!faceImage)
        {
            var imgGO = new GameObject("FaceImage", typeof(RectTransform), typeof(Image));
            faceImage = imgGO.transform;
            faceImage.SetParent(faceCanvas, false);
            ((RectTransform)faceImage).anchoredPosition = Vector2.zero;
        }

        return faceImage.GetComponent<Image>();
    }

    private static Transform FindFirstByNameContains(Transform root, string needle)
    {
        if (string.IsNullOrEmpty(needle)) return null;
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name.IndexOf(needle, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return t;
        return null;
    }
}
