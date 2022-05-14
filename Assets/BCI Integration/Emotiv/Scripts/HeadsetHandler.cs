using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System;

/// <summary>
/// In progress - handles headset connection
/// </summary>
public class HeadsetHandler : MonoBehaviour
{
    DataProcessing data = DataProcessing.Instance;
    BCITraining training;

    public DisplayBar displayBar;

    string headsetID;

    void Start()
    {
        training = new BCITraining();
        training.Init();
        data.onHeadsetChange += OnHeadsetChanged;
        data.HeadsetConnected += OnConnect;
        data.HeadsetConnectFail += OnConnectFail;
        TrainingHandler.Instance.QueryProfileOK += OnProfileQuery;
    }

    private void OnDestroy()
    {
        data.onHeadsetChange -= OnHeadsetChanged;
        data.HeadsetConnected -= OnConnect;
        data.HeadsetConnectFail -= OnConnectFail;
    }

    private void OnHeadsetChanged(object sender, EventArgs args)
    {
        if(data.GetHeadsetList().Count == 0)
        {
            print("No Headsets detected");
            return;
        }

        foreach(var item in data.GetHeadsetList())
        {
            if (item.Value == null)
            {
                print("headset list item value was null");
                continue;
            }

            //print($"Headset Detected!   ID: ${item.Value.HeadsetID}");
            headsetID = item.Value.HeadsetID;
        }
    }

    private void OnConnect(object sender, string args)
    {
        print("====== HEADSET CONNECTION SUCCESS");
    }

    private void OnConnectFail(object sender, string args)
    {
        print("====== HEADSET CONNECTION FAILURE");
    }

    public void ConnectionTest()
    {
        List<string> dataStreamList = new List<string>() { DataStreamName.SysEvents, DataStreamName.MentalCommands };
        DataStreamManager.Instance.StartDataStream(dataStreamList, headsetID);

        DataStreamManager.Instance.MentalCommandReceived += OnMentalCommandRecieved;
    }

    public void LoadProfile(string profileName)
    {
        training.QueryProfile();
        profileName = "test";
        training.LoadProfile(profileName);

    }

    private void OnMentalCommandRecieved(object sender, MentalCommandEventArgs args)
    {
            print("------------------------------------------------");
        print("------------------------------------------------");
        print("--------------MentalCommandRecieved---------------");
        print($"Act: {args.Act}, Power: {args.Pow}");
        print("------------------------------------------------");
        print("------------------------------------------------");
        CommandRecievedWrapper(args.Act, (float)args.Pow);
    }

    private void CommandRecievedWrapper(string s, float f)
    {
        displayBar.displayText = s;
        displayBar.progress = f;
    }

    private void OnProfileQuery(object sender, List<string> args)
    {
        print("------------------------------------------------");
        foreach (var i in training.ProfileLists)
            print(i);
        print("------------------------------------------------");
    }
}
