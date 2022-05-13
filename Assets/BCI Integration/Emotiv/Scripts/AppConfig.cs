using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores API parameters
/// </summary>
static class AppConfig
{
    public static string AppUrl = "wss://localhost:6868";
    public static string AppName = "UnityApp";

    /// <summary>
    /// Name of directory where contain tmp data and logs file.
    /// </summary>
    public static string TmpAppDataDir = "UnityApp";
    public static string ClientId = "rsa8GBfSBf1sc9q5FKqv5YBzXK1Lv80ZHO2XXO3z";
    public static string ClientSecret = "ENqLDk0ihytkaV8ERob6D6aK85xrejTCqMKFn1wEkosbfbPaOB9B6L3CYqrB2bOcmeZcrTgeKj4R5HDlqlSBJaDJkNw3zp0Xka4r97SYNcdW6ljRAHzgvGtcLWS4zwpj";
    public static string AppVersion = "3.2.0 Dev";

    /// <summary>
    /// License Id is used for App
    /// In most cases, you don't need to specify the license id. Cortex will find the appropriate license based on the client id
    /// </summary>
    public static string AppLicenseId = "";
}
