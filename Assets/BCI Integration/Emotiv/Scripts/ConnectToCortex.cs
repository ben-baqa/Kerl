using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System.IO;

/// <summary>
/// Initializes the connection to the Cortex API and drives updates to DataProcessor
/// </summary>
public class ConnectToCortex : MonoBehaviour
{
    // logs debug prints to an external file
    Logger logger = Logger.Instance;

    public bool printLogs;
    public bool printDataStreamsLogs;

    // using awake so that this is the first thing called (could also use execution order)
    private void Awake()
    {
        logger.Init();

        Cortex.Start(printLogs, printDataStreamsLogs);
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        Cortex.Stop();
    }

    string EmotivAppslicationPath()
    {
        string path = Application.dataPath;
        string newPath = "";
        // Debug.Log("ConnectToCortex: applicationPath: before " + path);
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            newPath = Path.GetFullPath(Path.Combine(path, @"../../"));
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            newPath = Path.GetFullPath(Path.Combine(path, @"../"));
        }
        // Debug.Log("ConnectToCortex: applicationPath: after " + newPath);
        return newPath;
    }
}
