using UnityEngine;

/// <summary>
/// Team index
/// </summary>
public enum Team {
    ONE = 0,
    TWO = 1
}

public static class Config {

    /// <summary>
    /// Board size value for X
    /// </summary>
    public const int COLUMNS = 6;

    /// <summary>
    /// Board size value for Y
    /// </summary>
    public const int ROWS = 8;

    /// <summary>
    /// Teams count value
    /// </summary>
    public const int TEAMS_COUNT = 2;

    /// <summary>
    /// Min units for battle
    /// </summary>
    public const int MIN_UNITS_COUNT = 2;

    /// <summary>
    /// Max units for battle
    /// </summary>
    public const int MAX_UNITS_COUNT = 5;

    /// <summary>
    /// Min damage value
    /// </summary>
    public const float MIN_DAMAGE_VALUE = 0.01f;

    /// <summary>
    /// Max damage hit
    /// </summary>
    public const float MAX_DAMAGE_VALUE = 0.12f;

    /// <summary>
    /// Offset between tiles
    /// </summary>
    public const float BOARD_OFFSET_VALUE = 1.3f;

    /// <summary>
    /// Color for team one
    /// </summary>
    public static Color TEAM_ONE_COLOR = Color.green;

    /// <summary>
    /// Color for team two
    /// </summary>
    public static Color TEAM_TWO_COLOR = Color.blue;

    /// <summary>
    /// Game loop step time
    /// </summary>
    public const float GAME_LOOP_INTERVAL = 0.5f;

    /// <summary>
    /// Color for full health
    /// </summary>
    public static Color HEALTH_HIGH_COLOR = Color.green;

    /// <summary>
    /// Color for half health
    /// </summary>
    public static Color HEALTH_MIDDLE_COLOR = Color.yellow;

    /// <summary>
    /// Color for 25% health
    /// </summary>
    public static Color HEALTH_LOW_COLOR = Color.red;   

    /// <summary>
    /// Min light power for stars
    /// </summary>
    public const float MIN_LIGHT_POWER = 0.03f;

    /// <summary>
    /// Max light power for stars
    /// </summary>
    public const float MAX_LIGHT_POWER = 0.1f;     
}