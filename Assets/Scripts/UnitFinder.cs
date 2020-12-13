using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitFinder {

    /// <summary>
    /// How many depth to search
    /// </summary>
    private const int MAX_FIND_DEPTH = (Config.COLUMNS > Config.ROWS) ? Config.COLUMNS : Config.ROWS;

    /// <summary>
    /// Link to units manager
    /// </summary>
    private UnitsManager _manager = null;

    /// <summary>
    /// Cache for coordinates
    /// </summary>
    private List<Vector2Int> _list = new List<Vector2Int> ();

    /// <summary>
    /// Cache for path point
    /// </summary>
    private Vector2Int _point = Vector2Int.zero;

    /// <summary>
    /// Init finder
    /// </summary>
    /// <param name="manager">Units manager</param>
    public void Init (UnitsManager manager) {
        _manager = manager;
    }

    /// <summary>
    /// Check enemy around coordinate
    /// </summary>
    /// <param name="coordinate">Start point to check</param>
    /// <param name="team">Team id</param>
    /// <returns>Unit controller</returns>
    public UnitController FindAround (Vector2Int coordinate, Team team) {
        FillListAround (coordinate);
        return FindUnit (team);
    }

    /// <summary>
    /// Find unit on board by coordinates list
    /// </summary>
    /// <returns>Controller</returns>
    private UnitController FindUnit (Team team) {
        UnitController unit = null;
        for (int i = 0; i < _list.Count; i++) {
            unit = _manager.GetUnitOnPosition (_list[i]);
            if (unit != null && unit.TeamId != team && unit.Status != UnitStatus.DEAD) {
                return unit;
            } else {
                unit = null;
            }
        }
        return unit;
    }

    /// <summary>
    /// Get coordinates around point with range mult
    /// Wave locator
    /// </summary>
    /// <param name="coordinate">Start point</param>
    /// <param name="range">Range mult</param>
    private void FillListAround (Vector2Int coordinate, int range = 1) {
        _list.Clear ();
        _list.Add (coordinate + Vector2Int.down * range + Vector2Int.left * range);
        _list.Add (coordinate + Vector2Int.down * range + Vector2Int.right * range);
        _list.Add (coordinate + Vector2Int.up * range + Vector2Int.right * range);
        _list.Add (coordinate + Vector2Int.up * range + Vector2Int.left * range);
        int distance = (int) Vector2Int.Distance (_list[0], _list[1]);
        for (int i = 0; i < distance - 1; i++) {
            _list.Add (_list[0] + Vector2Int.right * (i + 1));
        }
        distance = (int) Vector2Int.Distance (_list[1], _list[2]);
        for (int i = 0; i < distance - 1; i++) {
            _list.Add (_list[1] + Vector2Int.up * (i + 1));
        }
        distance = (int) Vector2Int.Distance (_list[2], _list[3]);
        for (int i = 0; i < distance - 1; i++) {
            _list.Add (_list[2] + Vector2Int.left * (i + 1));
        }
        distance = (int) Vector2Int.Distance (_list[3], _list[0]);
        for (int i = 0; i < distance - 1; i++) {
            _list.Add (_list[3] + Vector2Int.down * (i + 1));
        }
    }

    /// <summary>
    /// Find enemy on whole board by search rounds lins
    /// </summary>
    /// <param name="coordinate">Start point to search</param>
    /// <param name="team">Team id</param>
    public UnitController FindEnemy (Vector2Int coordinate, Team team) {
        UnitController unit = null;
        for (int i = 2; i < MAX_FIND_DEPTH; i++) {
            FillListAround (coordinate, i);
            unit = FindUnit (team);
            if (unit != null) {
                return unit;
            }
        }
        return unit;
    }

    /// <summary>
    /// Find path to enemy
    /// </summary>
    /// <param name="unit">Current unit board item</param>
    /// <param name="enemy">Enemy board item</param>
    /// <param name="board">Board</param>
    /// <returns>List of coordinates</returns>
    public List<Vector2Int> FindPath (BoardItem unit, BoardItem enemy, BoardItem[, ] board) {
        for (int i = 0; i < Config.COLUMNS; i++) {
            for (int j = 0; j < Config.ROWS; j++) {
                if (board[i, j].value >= 0) {
                    board[i, j].value = (int) BoardItemValue.FREE;
                }
            }
        }
        int x = 0;
        int y = 0;
        int value = 0;
        int step = 0;
        board[unit.x, unit.y].value = (int) BoardItemValue.FREE;
        board[enemy.x, enemy.y].value = 0;
        if (unit.x - 1 >= 0) {
            if (board[unit.x - 1, unit.y].value == (int) BoardItemValue.OBSTACLE) {
                step++;
            }
        } else {
            step++;
        }
        if (unit.y - 1 >= 0) {
            if (board[unit.x, unit.y - 1].value == (int) BoardItemValue.OBSTACLE) {
                step++;
            }
        } else {
            step++;
        }
        if (unit.x + 1 < Config.COLUMNS) {
            if (board[unit.x + 1, unit.y].value == (int) BoardItemValue.OBSTACLE) {
                step++;
            }
        } else {
            step++;
        }
        if (unit.y + 1 < Config.ROWS) {
            if (board[unit.x, unit.y + 1].value == (int) BoardItemValue.OBSTACLE) {
                step++;
            }
        } else {
            step++;
        }
        if (step == 4) {
#if DEV
            _point.Set (unit.x, unit.y);
            UnitController controller = _manager.GetUnitOnPosition (_point);
            Logger.Info (string.Format ("Unit #{0} from team {1} is locked", controller.UnitId, controller.TeamId));
#endif
            return null;
        } else {
            step = 0;
        }
        while (true) {
            for (y = 0; y < Config.ROWS; y++) {
                for (x = 0; x < Config.COLUMNS; x++) {
                    if (board[x, y].value == step) {
                        if (x - 1 >= 0) {
                            if (board[x - 1, y].value == (int) BoardItemValue.FREE) {
                                value = step + 1;
                                board[x - 1, y].value = value;
                            }
                        }
                        if (y - 1 >= 0)
                            if (board[x, y - 1].value == (int) BoardItemValue.FREE) {
                                value = step + 1;
                                board[x, y - 1].value = value;
                            }

                        if (x + 1 < Config.COLUMNS)
                            if (board[x + 1, y].value == (int) BoardItemValue.FREE) {
                                value = step + 1;
                                board[x + 1, y].value = value;
                            }

                        if (y + 1 < Config.ROWS)
                            if (board[x, y + 1].value == (int) BoardItemValue.FREE) {
                                value = step + 1;
                                board[x, y + 1].value = value;
                            }
                    }
                }
            }
            step++;
            if (board[unit.x, unit.y].value != (int) BoardItemValue.FREE) {
                break;
            }
            if (step != value || step > Config.COLUMNS * Config.ROWS) {
#if DEV
                _point.Set (unit.x, unit.y);
                UnitController controller = _manager.GetUnitOnPosition (_point);
                Logger.Info (string.Format ("Unit #{0} from team {1} can't find path!", controller.UnitId, controller.TeamId));
#endif
                return null;
            }
        }
        _list.Clear ();
        x = unit.x;
        y = unit.y;
        step = board[x, y].value;
        while (x != enemy.x || y != enemy.y) {
            if (x - 1 >= 0) {
                if (board[x - 1, y].value >= 0) {
                    if (board[x - 1, y].value < step) {
                        step = board[x - 1, y].value;
                        _point.Set (x - 1, y);
                        _list.Add (_point);
                        x--;
                        continue;
                    }
                }
            }
            if (y - 1 >= 0) {
                if (board[x, y - 1].value >= 0) {
                    if (board[x, y - 1].value < step) {
                        step = board[x, y - 1].value;
                        _point.Set (x, y - 1);
                        _list.Add (_point);
                        y--;
                        continue;
                    }
                }
            }
            if (x + 1 < Config.COLUMNS) {
                if (board[x + 1, y].value >= 0) {
                    if (board[x + 1, y].value < step) {
                        step = board[x + 1, y].value;
                        _point.Set (x + 1, y);
                        _list.Add (_point);
                        x++;
                        continue;
                    }
                }
            }
            if (y + 1 < Config.ROWS) {
                if (board[x, y + 1].value >= 0) {
                    if (board[x, y + 1].value < step) {
                        step = board[x, y + 1].value;
                        _point.Set (x, y + 1);
                        _list.Add (_point);
                        y++;
                        continue;
                    }
                }
            }
#if DEV
            _point.Set (unit.x, unit.y);
            UnitController controller = _manager.GetUnitOnPosition (_point);
            Logger.Info (string.Format ("Unit #{0} from team {1} can't find path from current point: {2}", controller.UnitId, controller.TeamId, controller.Coordinate));
            for (int u = 0; u < Config.ROWS; u++) {
                for (int h = 0; h < Config.COLUMNS; h++) {
                    Logger.Info (string.Format ("{0}x{1} = {2}", h, u, board[h, u].value));
                }
            }
#endif            
            return null;
        }
        return _list;
    }

}