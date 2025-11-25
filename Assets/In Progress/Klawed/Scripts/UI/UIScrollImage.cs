using UnityEngine;
using UnityEngine.UI;

public class UIScrollImage : MonoBehaviour
{
    public float speedX = 0.1f;
    public float speedY = 0f;

    RawImage rawImage;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        rawImage.uvRect = new Rect(
            rawImage.uvRect.x + speedX * Time.deltaTime,
            rawImage.uvRect.y + speedY * Time.deltaTime,
            rawImage.uvRect.width,
            rawImage.uvRect.height
        );
    }
}
