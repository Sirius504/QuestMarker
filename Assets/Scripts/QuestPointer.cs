using TMPro;
using UnityEngine;

public class QuestPointer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private float borderMargin = 0.1f;
    [SerializeField] private Vector2 defaultPivot;

    private RectTransform rectTransform;

    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Refresh(Vector3 screenPosition, float distance)
    {
        Vector2 screenPositionNormilized = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
        float xPivot = GetPivotValue(screenPositionNormilized.x, defaultPivot.x, borderMargin);
        float yPivot = GetPivotValue(screenPositionNormilized.y, defaultPivot.y, borderMargin);
        rectTransform.pivot = new Vector2(xPivot, yPivot);

        distanceText.text = $"{Mathf.RoundToInt(distance)}м";
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
}
