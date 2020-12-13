using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour {

    /// <summary>
    /// Cache transform
    /// </summary>
    private Transform _transform = null;

    /// <summary>
    /// Position move to
    /// </summary>
    private Vector3 _position = Vector3.zero;

    /// <summary>
    /// Constructor
    /// </summary>
    private void Awake () {
        _transform = transform;
    }

    /// <summary>
    /// Move to position
    /// </summary>
    /// <param name="position">Position to move</param>
    /// <param name="fast">Set without animation</param>
    public void MoveTo (Vector3 position, bool fast = false) {
        if (fast) {
            _transform.position = position;
        }
        _position = position;
        StartCoroutine (Moving ());
    }

    /// <summary>
    /// Moving coroutine
    /// </summary>
    IEnumerator Moving () {
        if (_position != _transform.position) {
            float timer = 0.0f;
            while (timer <= 1.0f) {
                _transform.position = Vector3.Lerp (_transform.position, _position, timer);
                timer += Time.deltaTime / Config.GAME_LOOP_INTERVAL / 2f;
                yield return null;
            }
            _transform.position = _position;
        }
        yield return null;
    }

}