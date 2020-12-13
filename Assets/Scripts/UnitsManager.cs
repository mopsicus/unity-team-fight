using System.Collections.Generic;
using UnityEngine;

public class UnitsManager {

    /// <summary>
    /// Unit objects for teams
    /// </summary>
    private GameObject[, ] _teamsUnits = new GameObject[Config.MAX_UNITS_COUNT, Config.TEAMS_COUNT];

    /// <summary>
    /// Unit controllers for teams
    /// </summary>
    private UnitController[, ] _teamsCountrollers = new UnitController[Config.MAX_UNITS_COUNT, Config.TEAMS_COUNT];

    /// <summary>
    /// Team count array
    /// </summary>
    private int[] _teamsCount = new int[Config.TEAMS_COUNT];

    /// <summary>
    /// Link to board controller
    /// </summary>
    private BoardController _board = null;

    /// <summary>
    /// Cache for dict with coordinate and controller
    /// </summary>
    private Dictionary<Vector2Int, UnitController> _unitsList = new Dictionary<Vector2Int, UnitController> (Config.MAX_UNITS_COUNT * Config.TEAMS_COUNT);

    /// <summary>
    /// Class to find units
    /// </summary>
    private UnitFinder _unitFinder = new UnitFinder ();

    /// <summary>
    /// List for path points
    /// </summary>
    private List<Vector2Int> _path = null;

    /// <summary>
    /// Build all units for battles
    /// </summary>
    /// <param name="prefab">Unit prefab</param>
    /// <param name="parent">Parent container</param>
    /// <param name="board">Board controller</param>
    public void Build (GameObject prefab, Transform parent, BoardController board) {
        _board = board;
        _unitFinder.Init (this);
        for (int i = 0; i < Config.MAX_UNITS_COUNT * Config.TEAMS_COUNT; i++) {
            GameObject unit = GameObject.Instantiate (prefab, Vector3.zero, Quaternion.identity, parent);
            unit.SetActive (false);
            int teamIndex = (i < Config.MAX_UNITS_COUNT) ? (int) Team.ONE : (int) Team.TWO;
            int index = (i < Config.MAX_UNITS_COUNT) ? i : i - Config.MAX_UNITS_COUNT;
            _teamsUnits[index, teamIndex] = unit;
            _teamsCountrollers[index, teamIndex] = unit.GetComponent<UnitController> ().Init (index, (Team) teamIndex);
        }
    }

    /// <summary>
    /// Init unit for new battle
    /// </summary>
    public void Init () {
        for (int i = 0; i < Config.MAX_UNITS_COUNT; i++) {
            for (int j = 0; j < Config.TEAMS_COUNT; j++) {
                _teamsUnits[i, j].SetActive (false);
            }
        }
        for (int i = 0; i < Config.TEAMS_COUNT; i++) {
            _teamsCount[i] = Random.Range (Config.MIN_UNITS_COUNT, Config.MAX_UNITS_COUNT + 1);
            for (int j = 0; j < _teamsCount[i]; j++) {
                _teamsUnits[j, i].SetActive (true);
            }
        }
        _unitsList.Clear ();
        for (int i = 0; i < Config.TEAMS_COUNT; i++) {
            SetUnits ((Team) i);
        }
    }

    /// <summary>
    /// Set unit for team
    /// </summary>
    /// <param name="team">Team index</param>
    private void SetUnits (Team team) {
        int teamIndex = (int) team;
        int count = _teamsCount[teamIndex];
#if DEV
        Logger.Info (string.Format ("Set {0} units for team: {1}", count, team));
#endif        
        Vector2Int[] coordinates = new Vector2Int[count];
        Vector3[] positions = new Vector3[count];
        for (int i = 0; i < count; i++) {
            coordinates[i] = _board.GetRandomCoordinate (team);
            positions[i] = _board.GetBoardItem (coordinates[i]).position;
            _teamsCountrollers[i, teamIndex].Reset ();
            _teamsCountrollers[i, teamIndex].SetPosition (coordinates[i], positions[i], true);
            _unitsList.Add (coordinates[i], _teamsCountrollers[i, teamIndex]);
        }
    }

    /// <summary>
    /// Remove unit from board
    /// </summary>
    /// <param name="coordinate">Coordinate</param>
    public void RemoveUnit (Vector2Int coordinate) {
        if (_unitsList.ContainsKey (coordinate)) {
            _unitsList.Remove (coordinate);
            _board.SetBoardItemStatus (coordinate, BoardItemValue.FREE);
        }
    }

    /// <summary>
    /// Get unit on coordinate
    /// </summary>
    /// <param name="coordinate">Target coordinate</param>
    /// <returns>Unit controller or null</returns>
    public UnitController GetUnitOnPosition (Vector2Int coordinate) {
        return (_unitsList.ContainsKey (coordinate)) ? _unitsList[coordinate] : null;
    }

    /// <summary>
    /// Set unit controller to new position
    /// </summary>
    public void SetUnitPosition (Vector2Int from, Vector2Int to) {
        _unitsList[to] = _unitsList[from];
        _unitsList[to].SetPosition (to, _board.GetBoardItem (to).position);
        _board.SetBoardItemStatus (to, BoardItemValue.FREE);
        RemoveUnit (from);
    }

    /// <summary>
    /// Make step for all units
    /// </summary>
    public void MakeStep () {
        for (int i = 0; i < Config.TEAMS_COUNT; i++) {
            for (int j = 0; j < _teamsCount[i]; j++) {
                DoStep (_teamsCountrollers[j, i]);
            }
        }
    }

    /// <summary>
    /// Get units list with controllers
    /// </summary>
    public Dictionary<Vector2Int, UnitController> UnitsList {
        get {
            return _unitsList;
        }
    }

    /// <summary>
    /// Set all teammates as obstacle for build right path
    /// </summary>
    /// <param name="team">Team index</param>
    private void SetTeamMatesAsObstacle (Team team) {
        int index = (int) team;
        for (int i = 0; i < _teamsCount[index]; i++) {
            _board.SetBoardItemStatus (_teamsCountrollers[i, index].Coordinate, BoardItemValue.OBSTACLE);
        }
    }

    /// <summary>
    /// Make unit step
    /// Check enemy near and attack if is exists
    /// Find enemy
    /// Move to enemy
    /// </summary>
    public void DoStep (UnitController unit) {
        if (unit.Status != UnitStatus.READY) {
            return;
        }
        UnitController enemy = _unitFinder.FindAround (unit.Coordinate, unit.TeamId);
        if (enemy != null) {
            float damage = Random.Range (Config.MIN_DAMAGE_VALUE, Config.MAX_DAMAGE_VALUE);
#if DEV
            Logger.Info (string.Format ("Unit #{0} from team {1} attacks unit #{2} from team {3} with damage = {4}", unit.UnitId, unit.TeamId, enemy.UnitId, enemy.TeamId, damage));
#endif          
            enemy.SetDamage (damage);
            if (enemy.Status == UnitStatus.DEAD) {
                RemoveUnit (enemy.Coordinate);
            }
            return;
        }
        enemy = _unitFinder.FindEnemy (unit.Coordinate, unit.TeamId);
        if (enemy == null) {
#if DEV
            Logger.Info (string.Format ("Unit #{0} from team {1} reports all enemy down", unit.UnitId, unit.TeamId));
#endif            
            unit.SetStatus (UnitStatus.FINISH);
            return;
        } else {
            SetTeamMatesAsObstacle (unit.TeamId);
            _path = _unitFinder.FindPath (_board.GetBoardItem (unit.Coordinate), _board.GetBoardItem (enemy.Coordinate), _board.FullBoard);
            if (_path != null) {
#if DEV
                Logger.Info (string.Format ("Unit #{0} from team {1} go to {2} from {3}", unit.UnitId, unit.TeamId, _path[0], unit.Coordinate));
#endif                
                SetUnitPosition (unit.Coordinate, _path[0]);
            } else {
#if DEV
                Logger.Info (string.Format ("Unit #{0} from team {1} has no path", unit.UnitId, unit.TeamId));
#endif              
                return;
            }
        }
    }

}