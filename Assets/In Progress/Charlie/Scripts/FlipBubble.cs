using UnityEngine;

public class FlipBubbleByRotation : MonoBehaviour
{
    [SerializeField] private Transform npcTransform; 
    [SerializeField] private RectTransform bubbleRect; 
    
    void Start()
    {
        if (bubbleRect == null)
            bubbleRect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (npcTransform == null || bubbleRect == null) return;

        float yRotation = npcTransform.eulerAngles.y;
        
        if (yRotation > 90 && yRotation < 270)
        {
            bubbleRect.localScale = new Vector3(-1, 1, 1); 
        }
        else
        {
            bubbleRect.localScale = new Vector3(1, 1, 1); 
        }
    }
}