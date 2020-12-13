using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Board item value type
/// </summary>
public enum BoardItemValue {

    ///<summary>
    /// Board item is free 
    /// </summary>
    FREE = -1,

    ///<summary>
    /// Board item obstacle
    /// </summary>
    OBSTACLE = -2
}

/// <summary>
/// Board struct for pathfinding and store positions
/// </summary>
public struct BoardItem {

    /// <summary>
    /// Position in world
    /// </summary>
    public Vector2 position;

    /// <summary>
    /// Tile value for pathfinding
    /// </summary>
    public int value;

    /// <summary>
    /// X coordinate
    /// </summary>
    public int x;

    /// <summary>
    /// Y coordinate
    /// </summary>
    public int y;
}

public class BoardController {

    /// <summary>
    /// Storage for tiles positions
    /// </summary>
    private BoardItem[, ] _positions = new BoardItem[Config.COLUMNS, Config.ROWS];

    /// <summary>
    /// Tile controller for manage stars alpha
    /// </summary>
    private TileController[, ] _tiles = new TileController[Config.COLUMNS, Config.ROWS];

    /// <summary>
    /// Picked cache
    /// </summary>
    private List<Vector2Int> _picked = new List<Vector2Int> (Config.MAX_UNITS_COUNT * Config.TEAMS_COUNT);

    /// <summary>
    /// Build board field with tiles
    /// </summary>
    /// <param name="prefab">Tile prefab</param>
    /// <param name="parent">Board parent container</param>
    public void Build (GameObject prefab, Transform parent) {
#if DEV
        Logger.Info (string.Format ("Create board {0}×{1} with tile: {2}", Config.COLUMNS, Config.ROWS, prefab.name.ToUpper ()));
#endif
        Vector2 offset = new Vector2 (Config.COLUMNS / 2f * Config.BOARD_OFFSET_VALUE, Config.ROWS / 2f * Config.BOARD_OFFSET_VALUE);
        for (int y = 0; y < Config.ROWS; y++) {
            for (int x = 0; x < Config.COLUMNS; x++) {
                _positions[x, y].position = new Vector2 (x * Config.BOARD_OFFSET_VALUE + Config.BOARD_OFFSET_VALUE / 2f, y * Config.BOARD_OFFSET_VALUE + Config.BOARD_OFFSET_VALUE / 2f) - offset;
                _positions[x, y].value = (int) BoardItemValue.FREE;
                _positions[x, y].x = x;
                _positions[x, y].y = y;
                GameObject tile = GameObject.Instantiate (prefab, _positions[x, y].position, Quaternion.identity, parent);
                _tiles[x, y] = tile.GetComponent<TileController> ();
                _tiles[x, y].Init ();
            }
        }
    }

    /// <summary>
    /// Get board item with data
    /// </summary>
    /// <param name="coordinates">Coordinates on field</param>
    public BoardItem GetBoardItem (Vector2Int coordinates) {
        return _positions[coordinates.x, coordinates.y];
    }

    /// <summary>
    /// Get random coordinate for unit
    /// </summary>
    /// <param name="team">Team index</param>
    /// <returns>Coordinate</returns>
    public Vector2Int GetRandomCoordinate (Team team) {
        Vector2Int coordinate = Vector2Int.zero;
        coordinate.x = -1;
        while (coordinate.x < 0 || _picked.Contains (coordinate)) {
            coordinate.x = (team == Team.ONE) ? Random.Range (0, Config.COLUMNS / 2) : Random.Range (Config.COLUMNS / 2, Config.COLUMNS);
            coordinate.y = Random.Range (0, Config.ROWS);
        }
        _picked.Add (coordinate);
        return coordinate;
    }

    /// <summary>
    /// Set value for board item
    /// </summary>
    /// <param name="coordinate">Coordinate to set</param>
    /// <param name="value">Free or obstacle</param>
    public void SetBoardItemStatus (Vector2Int coordinate, BoardItemValue value) {
        _positions[coordinate.x, coordinate.y].value = (int) value;
    }

    /// <summary>
    /// Clear all vars for new battle
    /// </summary>
    public void Reset () {
        _picked.Clear ();
        for (int y = 0; y < Config.ROWS; y++) {
            for (int x = 0; x < Config.COLUMNS; x++) {
                _positions[x, y].value = (int) BoardItemValue.FREE;
                _tiles[x, y].Init ();
            }
        }
    }

    /// <summary>
    /// Get whole board
    /// </summary>
    public BoardItem[, ] FullBoard {
        get {
            return _positions;
        }
    }

}