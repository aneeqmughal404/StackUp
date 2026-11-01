using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea = Rect.zero;
    private Vector2 lastScreenSize = Vector2.zero;
    private ScreenOrientation lastOrientation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        RefreshSafeArea();
    }

    private void Update()
    {
        // Continuously checks for screen changes (e.g., rotation, split-screen)
        if (Screen.safeArea != lastSafeArea || 
            new Vector2(Screen.width, Screen.height) != lastScreenSize || 
            Screen.orientation != lastOrientation)
        {
            RefreshSafeArea();
        }
    }

    private void RefreshSafeArea()
    {
        // Cache the current states
        lastSafeArea = Screen.safeArea;
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        lastOrientation = Screen.orientation;

        // Convert pixel safe area bounds to normalized 0-1 anchors
        Vector2 anchorMin = lastSafeArea.position;
        Vector2 anchorMax = lastSafeArea.position + lastSafeArea.size;

        anchorMin.x /= lastScreenSize.x;
        anchorMin.y /= lastScreenSize.y;
        anchorMax.x /= lastScreenSize.x;
        anchorMax.y /= lastScreenSize.y;

        // Apply normalized coordinates to the RectTransform
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}