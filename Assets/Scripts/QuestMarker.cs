using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class QuestMarker : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Canvas canvas;
    [SerializeField] private QuestPointer markerPrefab;

    private Camera camera;
    private QuestPointer marker;
    private Vector3 targetScreenPosition;
    private RectTransform markerRect;

    // Start is called before the first frame update
    private void Start()
    {
        camera = GetComponent<Camera>();
        marker = Instantiate(markerPrefab, canvas.transform);
        markerRect = marker.GetComponent<RectTransform>();
        markerRect.anchorMin = markerRect.anchorMax = Vector2.zero;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        DrawMarker();
    }
    
    private void DrawMarker()
    {
        Vector3 toTarget = target.position - camera.transform.position;
        markerRect.position = GetMarkerScreenPosition(toTarget);
        marker.Refresh(markerRect.position, toTarget.magnitude);
    }

    private Vector3 GetMarkerScreenPosition(Vector3 toTarget)
    {
        var toTargetLocal = camera.transform.InverseTransformVector(toTarget);
        var spherical = CartesianToSpherical(toTargetLocal);

        Vector2 cameraAngleConstraints = GetCameraAngleConstraints();
        var resultY = Mathf.Clamp(spherical.y, -cameraAngleConstraints.x, cameraAngleConstraints.x);
        var resultZ = Mathf.Clamp(spherical.z, -cameraAngleConstraints.y, cameraAngleConstraints.y);
        var constrained = new Vector3(spherical.x, resultY, resultZ);
        
        Vector3 resultLocal = SphericalToCortesian(constrained);

        var result = camera.transform.TransformDirection(resultLocal);

        var screenPosition = camera.WorldToScreenPoint(result + camera.transform.position);
        screenPosition = ClampInScreen(screenPosition);
        return screenPosition;
    }

    private Vector3 ClampInScreen(Vector3 screenPosition)
    {
        float x = Mathf.Clamp(screenPosition.x, 0f, Screen.width);
        float y = Mathf.Clamp(screenPosition.y, 0f, Screen.height);
        return new Vector3(x, y, screenPosition.z);
    }

    private Vector2 GetCameraAngleConstraints()
    {
        float verticalConstraint = camera.fieldOfView / 2;
        float horizontalConstraint = Mathf.Atan(Mathf.Tan(verticalConstraint * Mathf.Deg2Rad) * camera.aspect) * Mathf.Rad2Deg;
        return new Vector2(verticalConstraint, horizontalConstraint);
    }

    private Vector3 CartesianToSpherical(Vector3 cartesian)
    {
        float radius = cartesian.magnitude;
        if (radius == 0)
            return Vector3.zero;
        float longitude = Mathf.Atan2(cartesian.x, cartesian.z);
        longitude *= Mathf.Rad2Deg;
        float lattitude = Mathf.Asin(cartesian.y / radius) * Mathf.Rad2Deg;
        return new Vector3(radius, lattitude, longitude);
    }

    private Vector3 SphericalToCortesian(Vector3 spherical)
    {
        var sphericalRad = new Vector3(spherical.x, spherical.y * Mathf.Deg2Rad, spherical.z * Mathf.Deg2Rad);
        float x = sphericalRad.x * Mathf.Sin(sphericalRad.z) * Mathf.Cos(sphericalRad.y);
        float y = sphericalRad.x * Mathf.Sin(Mathf.Abs(sphericalRad.y)) * Mathf.Sign(sphericalRad.y);
        float z = sphericalRad.x * Mathf.Cos(sphericalRad.z) * Mathf.Cos(sphericalRad.y);
        return new Vector3(x, y, z);
    }
}
