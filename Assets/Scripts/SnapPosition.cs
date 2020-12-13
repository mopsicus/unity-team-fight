using UnityEngine;

/// <summary>
/// Fix position for objects in non canvas
/// </summary>
[ExecuteInEditMode]
public class SnapPosition : MonoBehaviour {

    /// <summary>
    /// Anchor settings
    /// </summary>
    private enum AnchorType {
        CENTER,
        MIDDLE_LEFT,
        MIDDLE_RIGHT,
        BOTTOM_CENTER,
        BOTTOM_LEFT,
        BOTTOM_RIGHT,
        TOP_LEFT,
        TOP_RIGHT,
        TOP_CENTER
    }

    /// <summary>
    /// Position anchor
    /// </summary>
    [SerializeField]
    private AnchorType Anchor = AnchorType.CENTER;

    /// <summary>
    /// Offset for position
    /// </summary>
    [SerializeField]
    private Vector2 Offset = Vector2.zero;

    /// <summary>
    /// Resolution cache
    /// </summary>
    private Vector2 _resolution = Vector2.zero;

    /// <summary>
    /// Camera cache
    /// </summary>
    private Camera _camera = null;

    /// <summary>
    /// Transform cache
    /// </summary>
    private Transform _transform = null;

    /// <summary>
    /// Init
    /// </summary>
    void Awake () {
        _transform = transform;
        _camera = Camera.main;
    }

    /// <summary>
    /// Set anchor
    /// </summary>
    Vector2 ScreenAnchor (AnchorType value) {
        switch (value) {
            case AnchorType.CENTER:
                return new Vector2 (Screen.width / 2f, Screen.height / 2f);
            case AnchorType.MIDDLE_LEFT:
                return new Vector2 (0.0f, Screen.height / 2f);
            case AnchorType.MIDDLE_RIGHT:
                return new Vector2 (Screen.width, Screen.height / 2f);
            case AnchorType.BOTTOM_CENTER:
                return new Vector2 (Screen.width / 2f, 0.0f);
            case AnchorType.BOTTOM_LEFT:
                return new Vector2 (0.0f, 0.0f);
            case AnchorType.BOTTOM_RIGHT:
                return new Vector2 (Screen.width, 0.0f);
            case AnchorType.TOP_CENTER:
                return new Vector2 (Screen.width / 2f, Screen.height);
            case AnchorType.TOP_LEFT:
                return new Vector2 (0.0f, Screen.height);
            case AnchorType.TOP_RIGHT:
                return new Vector2 (Screen.width, Screen.height);
        }
        return Vector2.zero;
    }

    /// <summary>
    /// Update and set transform position
    /// Fix Z position for 2D
    /// </summary>
    void UpdatePosition () {
        Vector2 point = ScreenAnchor (Anchor);
        Vector3 position = _camera.ScreenToWorldPoint (point + Offset);
        position.z = 0.0f;
        _transform.position = position;
    }

    /// <summary>
    /// Update if resolution changed
    /// </summary>
    void LateUpdate () {
        if (_resolution.x != Screen.width || _resolution.y != Screen.height) {
            UpdatePosition ();
            _resolution.x = Screen.width;
            _resolution.y = Screen.height;
        }

#if UNITY_EDITOR
        UpdatePosition ();
#endif
    }

}