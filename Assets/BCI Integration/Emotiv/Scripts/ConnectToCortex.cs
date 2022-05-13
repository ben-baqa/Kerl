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
    DataStreamManager dataStream = DataStreamManager.Instance;
    DataProcessing dataProcessor = DataProcessing.Instance;

    // logs debug prints to an external file
    Logger logger = Logger.Instance;

    // drives timed updates to data
    float contactQualityQueryTimer = 0;
    float headsetQueryTimer = 0;

    public float contactQualityInterval = 0.5f;
    public float headsetQueryInterval = 2;

    private void Start()
    {
        //logger.Init();

        dataStream.SetAppConfig(AppConfig.ClientId, AppConfig.ClientSecret,
                                AppConfig.AppVersion, AppConfig.AppName,
                                AppConfig.TmpAppDataDir, AppConfig.AppUrl,
                                EmotivAppslicationPath());

        dataStream.StartAuthorize(AppConfig.AppLicenseId);
    }

    private void Update()
    {
        // update contact quality data
        contactQualityQueryTimer += Time.deltaTime;
        if(contactQualityQueryTimer > contactQualityInterval)
        {
            contactQualityQueryTimer -= contactQualityInterval;
            dataProcessor.updateContactQuality();
        }

        // update headset data
        headsetQueryTimer += Time.deltaTime;
        if(headsetQueryTimer > headsetQueryInterval)
        {
            headsetQueryTimer -= headsetQueryInterval;
            dataProcessor.Process();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        dataStream.Stop();
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
