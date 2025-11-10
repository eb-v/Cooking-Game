using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SmoothXMoveWrap : MonoBehaviour
{
    public RectTransform target;     // leave empty
    public float speed = 250f;       // pixels per second
    public float maxX = 2500f;  
    public float resetX = -1000f;  
    public bool toRight = true;      // true = right false = left
    public bool useUnscaledTime = false;

    Vector2 pos;
    float dt => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

    void Awake()
    {
        if (!target) target = GetComponent<RectTransform>();
        pos = target.anchoredPosition;
    }

    void Update()
    {
        float dir = toRight ? 1f : -1f;
        pos.x += dir * speed * dt;

        if (toRight)
        {
            if (pos.x >= maxX) pos.x = resetX;   
        }
        else
        {
            if (pos.x <= -maxX) pos.x = resetX;  
        }

        target.anchoredPosition = pos;
    }
}
