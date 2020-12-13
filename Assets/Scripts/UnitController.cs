using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit status
/// </summary>
public enum UnitStatus {

    /// <summary>
    /// Unit is ready to move and fight
    /// </summary>
    READY = 0,

    /// <summary>
    /// Unit finished game
    /// </summary>
    FINISH = 1,

    /// <summary>
    /// Unit has been killed
    /// </summary>
    DEAD = 2
}

public class UnitController : MonoBehaviour {

    /// <summary>
    /// Sprite to set team color
    /// </summary>
    [SerializeField]
    private SpriteRenderer Background = null;

    /// <summary>
    /// Link to health controller
    /// </summary>
    private HealthBarController _health = null;

    /// <summary>
    /// Current coordinate on field
    /// </summary>
    private Vector2Int _coordinate = Vector2Int.zero;

    /// <summary>
    /// Team index value
    /// </summary>
    private Team _teamIndex = Team.ONE;

    /// <summary>
    /// Unit index value
    /// </summary>
    private int _unitIndex = -1;

    /// <summary>
    /// Current health value
    /// </summary>
    private float _currentHealth = 1f;

    /// <summary>
    /// Flag to check unit status
    /// </summary>
    private UnitStatus _status = UnitStatus.READY;

    /// <summary>
    /// Link to animator script
    /// </summary>
    private UnitAnimator _animator = null;

    /// <summary>
    /// Constructor
    /// </summary>
    private void Awake () {
        _health = GetComponentInChildren<HealthBarController> ();
        _animator = GetComponent<UnitAnimator> ();
    }

    /// <summary>
    /// Init unit
    /// </summary>
    /// <param name="id">Unit id</param>
    /// <param name="team">Team id</param>
    public UnitController Init (int id, Team team) {
        _unitIndex = id;
        _teamIndex = team;
        Background.color = (team == Team.ONE) ? Config.TEAM_ONE_COLOR : Config.TEAM_TWO_COLOR;
        Reset ();
        return this;
    }

    /// <summary>
    /// Set unit position
    /// </summary>
    /// <param name="coordinate">Coordinate on field</param>
    /// <param name="position">Position on field</param>
    /// <param name="fast">Set without animation</param>
    public void SetPosition (Vector2Int coordinate, Vector3 position, bool fast = false) {
#if DEV
        Logger.Info (string.Format ("Unit #{0} from team {1} moved to {2}", _unitIndex, _teamIndex, coordinate));
#endif         
        _coordinate = coordinate;
        _animator.MoveTo (position, fast);
    }

    /// <summary>
    /// Reset vars for new battle
    /// </summary>
    public void Reset () {
        _currentHealth = 1f;
        _health.Set (_currentHealth);
        SetStatus (UnitStatus.READY);
    }

    /// <summary>
    /// Set damage to unit
    /// </summary>
    /// <param name="value">Damage value</param>
    public void SetDamage (float value) {
        _currentHealth -= value;
        _health.Set (_currentHealth);
        CheckForDead ();
    }

    /// <summary>
    /// Set unit status
    /// </summary>
    public void SetStatus (UnitStatus status) {
        _status = status;
    }

    /// <summary>
    /// Check unit health and disable it if dead
    /// </summary>
    private void CheckForDead () {
        if (_currentHealth <= 0.0f) {
            _status = UnitStatus.DEAD;
            gameObject.SetActive (false);
#if DEV
            Logger.Info (string.Format ("Unit #{0} from team {1} is dead. R.I.P.", _unitIndex, _teamIndex));
#endif              
        }
    }

    /// <summary>
    /// Unit id
    /// </summary>
    public int UnitId {
        get {
            return _unitIndex;
        }
    }

    /// <summary>
    /// Team
    /// </summary>
    public Team TeamId {
        get {
            return _teamIndex;
        }
    }

    /// <summary>
    /// Unit coordinate
    /// </summary>
    public Vector2Int Coordinate {
        get {
            return _coordinate;
        }
    }

    /// <summary>
    /// Current unit status
    /// Wait for next step
    /// Finish game (no enemies)
    /// Killed with honor
    /// </summary>
    public UnitStatus Status {
        get {
            return _status;
        }
    }

}