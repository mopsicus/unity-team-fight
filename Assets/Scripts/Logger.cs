using System;
using UnityEngine;

/// <summary>
/// Debug logger
/// </summary>
public static class Logger {

    /// <summary>
    /// Log info message
    /// </summary>
    /// <param name="data">Object to log</param>
    public static void Info (object data) {
        Debug.LogFormat ("{0}  {1}", DateTime.Now.ToString ("dd.MM.yy hh:mm:ss ffff"), data);
    }

    /// <summary>
    /// Log error message
    /// </summary>
    /// <param name="data">Object to log</param>
    public static void Error (object data) {
        Debug.LogErrorFormat ("{0}  {1}", DateTime.Now.ToString ("dd.MM.yy hh:mm:ss ffff"), data);
    }

}