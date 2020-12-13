using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    /// <summary>
    /// Delay before action begin
    /// </summary>
    private const int START_DELAY = 1;

    /// <summary>
    /// Prefab object for board tiles
    /// </summary>
    [SerializeField]
    private GameObject TilePrefab = null;

    /// <summary>
    /// Prefab object for units
    /// </summary>
    [SerializeField]
    private GameObject UnitPrefab = null;

    /// <summary>
    /// Board container transform
    /// </summary>
    [SerializeField]
    private Transform BoardTransform = null;

    /// <summary>
    /// Link to UI controller
    /// </summary>
    [SerializeField]
    private UIController _uiController = null;

    /// <summary>
    /// Link to board controller
    /// </summary>
    private BoardController _boardController = new BoardController ();

    /// <summary>
    /// Link to units manager
    /// </summary>
    private UnitsManager _unitsManager = new UnitsManager ();

    /// <summary>
    /// Waiter cache to prevent alloc
    /// </summary>
    private WaitForSeconds _waiter = new WaitForSeconds (Config.GAME_LOOP_INTERVAL);

    /// <summary>
    /// Game step count
    /// </summary>
    private int _step = 0;

    /// <summary>
    /// Constructor
    /// Game entry point
    /// </summary>
    void Start () {
        _uiController.SwitchButton (true);
        _boardController.Build (TilePrefab, BoardTransform);
        _unitsManager.Build (UnitPrefab, BoardTransform, _boardController);
    }

    /// <summary>
    /// Begin new battle
    /// </summary>
    public void BeginBattle () {
        _uiController.SwitchButton (false);
        _boardController.Reset ();
        _unitsManager.Init ();
        StartBattle ();
    }

    /// <summary>
    /// Start new battle loop
    /// </summary>
    private void StartBattle () {
#if DEV
        Logger.Info ("New battle started");
#endif          
        StartCoroutine (GameLoop ());
    }

    /// <summary>
    /// Coroutine for game loop
    /// </summary>
    private IEnumerator GameLoop () {
        while (true) {
            _step++;
            if (_step > START_DELAY) {
                _unitsManager.MakeStep ();
                if (CheckGameOver ()) {
                    yield break;
                }
            }
#if DEV
            Logger.Info (string.Format ("------------------------- step #{0} -------------------------", _step));
#endif               
            yield return _waiter;
        }
    }

    private bool CheckGameOver () {
        int count = 0;
        int units = _unitsManager.UnitsList.Values.Count;
        Team team = Team.ONE;
        foreach (UnitController unit in _unitsManager.UnitsList.Values) {
            if (unit.Status == UnitStatus.FINISH) {
                team = unit.TeamId;
                count++;
            }
        }
        bool result = (count == units);
        if (result) {
            _uiController.SwitchButton (true);
            _uiController.ShowResult (team);
            _step = 0;
#if DEV
            Logger.Info (string.Format ("Game over! Team {0} wins!", team));
#endif 
        }
        return result;
    }

    public void Test () {
        for (int y = 0; y < Config.ROWS; y++) {
            for (int x = 0; x < Config.COLUMNS; x++) {
                Logger.Info (string.Format ("{0}x{1} = {2}", x, y, _boardController.GetBoardItem (new Vector2Int (x, y)).value));
            }
        }
    }

}