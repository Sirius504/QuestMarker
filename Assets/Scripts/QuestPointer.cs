using System;
using TMPro;
using UnityEngine;


public class QuestPointer : MonoBehaviour
{
    private class PointerParameters
    {
        public Vector2 pivot;
        public Vector2 arrowAnchors;
        public float arrowRotation;
    }

    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private Vector2 borderMarginLeftRight;
    [SerializeField] private Vector2 borderMarginTopBottom;
    [SerializeField] private Vector2 defaultPivot;
    [SerializeField] private Vector2 defaultArrowAnchors;
    [SerializeField] private RectTransform arrowRect;
    [SerializeField] private RectTransform arrowSpriteRect;

    private RectTransform rectTransform;

    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    // if marker position is close to border (inside border margin), interpolate
    // it's position from default value to normilized screen position (it gives exact pivot we need)
    // based on distance to border
    public void Refresh(Vector3 screenPosition, float distance)
    {
        Vector2 screenPositionNormilized = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
        // two margins closest to our pointer
        Vector2 margins = GetNearestMargins(screenPositionNormilized);
        RefreshPivot(screenPositionNormilized, margins);
        RefreshArrow(screenPositionNormilized, margins);

        distanceText.text = $"{Mathf.RoundToInt(distance)}м";
    }

    private void RefreshPivot(Vector2 screenPositionNormilized, Vector2 margins)
    {
        float xPivot = GetPivotValue(screenPositionNormilized.x, defaultPivot.x, margins.x);
        float yPivot = GetPivotValue(screenPositionNormilized.y, defaultPivot.y, margins.y);
        rectTransform.pivot = new Vector2(xPivot, yPivot);
    }

    private void RefreshArrow(Vector2 screenPositionNormilized, Vector2 margins)
    {
        var newAnchors = GetNewArrowAnchors(screenPositionNormilized, margins);
        arrowRect.anchorMax = arrowRect.anchorMin = newAnchors;
        arrowRect.pivot = newAnchors;
        arrowRect.anchoredPosition = Vector2.zero;
        arrowSpriteRect.rotation = GetArrowRotation(screenPositionNormilized, margins);
    }

    private Quaternion GetArrowRotation(Vector2 screenPositionNormilized, Vector2 margins)
    {
        bool inXMargin = InMargin(screenPositionNormilized.x, margins.x);
        bool inYMargin = InMargin(screenPositionNormilized.y, margins.y);

        if (!inXMargin && !inYMargin)
            return Quaternion.identity;
        float rotationDegrees = 0.0f;
        if (inXMargin && inYMargin)
            rotationDegrees = screenPositionNormilized.y <= 0.5f ? 45.0f : 135.0f;
        else if (inXMargin)
            rotationDegrees = 90.0f;
        else if (inYMargin)
            rotationDegrees = screenPositionNormilized.y <= 0.5f ? 0f : 180.0f;

        rotationDegrees *= Mathf.Sign(screenPositionNormilized.x - 0.5f);
        return Quaternion.Euler(0f, 0f, rotationDegrees);
    }

    private Vector2 GetNewArrowAnchors(Vector2 screenPositionNormilized, Vector2 margins)
    {
        Vector2 newAnchors = defaultArrowAnchors;
        bool inXMargin = InMargin(screenPositionNormilized.x, margins.x);
        bool inYMargin = InMargin(screenPositionNormilized.y, margins.y);

        newAnchors = inXMargin ^ inYMargin ? new Vector2(0.5f, 0.5f) : newAnchors;
        newAnchors.x = inXMargin ? Mathf.Round(screenPositionNormilized.x) : newAnchors.x;
        newAnchors.y = inYMargin ? Mathf.Round(screenPositionNormilized.y) : newAnchors.y;
        return newAnchors;
    }

    private float GetPivotValue(float screenPositionNormilized, float defaultValue, float margin)
    {
        float distance = GetDistanceToBorder(screenPositionNormilized);
        return distance < margin
             ? Mathf.Lerp(defaultValue, screenPositionNormilized, (margin - distance) / margin)
             : defaultValue;
    }
    
    private float GetDistanceToBorder(float screenPositionNormilized)
    {
        var result = Mathf.Abs(screenPositionNormilized - Mathf.Round(screenPositionNormilized));
        return result;
    }

    private bool InMargin(float screenPositionNormilized, float margin)
    {
        float distance = GetDistanceToBorder(screenPositionNormilized);
        return distance < margin;
    }

    private Vector2 GetNearestMargins(Vector2 screenPositionNormilized)
    {
        float xMargin = screenPositionNormilized.x <= 0.5f
                ? borderMarginLeftRight.x
                : borderMarginLeftRight.y;
        float yMargin = screenPositionNormilized.y <= 0.5f
                ? borderMarginTopBottom.y
                : borderMarginTopBottom.x;
        return new Vector2(xMargin, yMargin);
    }
}
