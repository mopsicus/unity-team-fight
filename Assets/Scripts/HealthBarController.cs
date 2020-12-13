using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour {

    /// <summary>
    /// Bar object
    /// </summary>
    [SerializeField]
    private Transform Bar = null;

    /// <summary>
    /// Link to filler for color change
    /// </summary>
    [SerializeField]
    private SpriteRenderer Filler = null;

    /// <summary>
    /// Cache for scaling
    /// </summary>
    private Vector3 _scale = Vector3.one;

    /// <summary>
    /// Contructor
    /// </summary>
    private void Awake () {
        _scale = Bar.localScale;
    }

    /// <summary>
    /// Set health bar value from 0 to 1
    /// </summary>
    /// <param name="value">Fill value</param>
    public void Set (float value) {
        if (value < 0f) {
            value = 0f;
        }
        _scale.x = value;
        Bar.localScale = _scale;
        if (value >= 0.75f) {
            Filler.color = Config.HEALTH_HIGH_COLOR;
        } else if (value >= 0.35f) {
            Filler.color = Config.HEALTH_MIDDLE_COLOR;
        } else {
            Filler.color = Config.HEALTH_LOW_COLOR;
        }
    }

}