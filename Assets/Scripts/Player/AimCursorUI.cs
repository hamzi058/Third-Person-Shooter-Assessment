using UnityEngine;

public class AimCursorUI : MonoBehaviour
{
    RectTransform rect;

    public PlayerMovement playerMovement;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (playerMovement.controlMode == PlayerMovement.ControlMode.PC)
        {
            Vector3 mousePos = Input.mousePosition;

            // ⭐ SAFETY CHECK
            if (float.IsInfinity(mousePos.x) ||
                float.IsInfinity(mousePos.y) ||
                float.IsNaN(mousePos.x) ||
                float.IsNaN(mousePos.y))
                return;

            rect.position = mousePos;
        }
        else
        {
            // ⭐ lock to centre on mobile
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
        }
    }
}