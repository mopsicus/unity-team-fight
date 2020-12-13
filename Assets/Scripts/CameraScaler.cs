using UnityEngine;

/// <summary>
/// Camera scaler ortho size by screen size
/// </summary>
public class CameraScaler : MonoBehaviour {

    /// <summary>
    /// Reference screen width
    /// </summary>
    [SerializeField]
    private float TargetWidth = 1080f;

    /// <summary>
    /// Reference screen height
    /// </summary>
    [SerializeField]
    private float TargetHeight = 1920f;

    /// <summary>
    /// Reference pixels per units
    /// </summary>
    [SerializeField]
    private int PixelsPerUnits = 100;

    /// <summary>
    /// Constructor
    /// </summary>
    private void Awake () {
        Camera camera = Camera.main;
        float desiredRatio = TargetWidth / TargetHeight;
        float currentRatio = (float) UnityEngine.Screen.width / (float) UnityEngine.Screen.height;
        if (currentRatio >= desiredRatio) {
            camera.orthographicSize = TargetHeight / 2f / PixelsPerUnits;
        } else {
            float differenceInSize = desiredRatio / currentRatio;
            camera.orthographicSize = TargetHeight / 2f / PixelsPerUnits * differenceInSize;
        }
    }

}