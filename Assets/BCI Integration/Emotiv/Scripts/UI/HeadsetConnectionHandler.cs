using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System;
using WebSocket4Net;
using Newtonsoft.Json.Linq;

public class HeadsetConnectionHandler : MonoBehaviour
{
    HeadsetFinder headsetFinder = HeadsetFinder.Instance;
    DataStreamManager data = DataStreamManager.Instance;
    BCITraining training;

    [SerializeField] GameObject headsetItemPrefab;
    [SerializeField] Transform headsetList;

    MentalCommand previousCommand;
    bool connected = false;

    List<Headset> headsets;
    bool newInfo = false;

    //WebSocket socket;
    //int socketRequestID = 0;

    void Start()
    {
        //training = new BCITraining();
        //training.Init();

        //data.onHeadsetChange += OnHeadsetChanged;
        //data.HeadsetConnected += OnConnect;

        data.QueryHeadsetOK += OnHeadsetChanged;
        //headsetFinder.QueryHeadsetOK += OnHeadsetChanged;

        //TrainingHandler.Instance.QueryProfileOK += OnProfileQuery;
    }
    private void OnDestroy()
    {
        //data.onHeadsetChange -= OnHeadsetChanged;
        //data.HeadsetConnected -= OnConnect;

        data.QueryHeadsetOK += OnHeadsetChanged;
        //headsetFinder.QueryHeadsetOK -= OnHeadsetChanged;
    }

    private void Update()
    {
        //var mentalCommandData = DataStreamManager.Instance.GetMentalCommands();
        //if (mentalCommandData == null)
        //    return;
        //foreach (var command in mentalCommandData)
        //{
        //    if (command != previousCommand)
        //        print(command);

        //    previousCommand = command;

        //    //displayBar.displayText = command.action;
        //    //displayBar.progress = command.power;
        //}

        //OnHeadsetChanged(this, data.detectedHeadsets);

        if (newInfo)
        {
            newInfo = false;
            ShowHeadsets();
        }
    }

    // called by the event system when there is a change in the list of available headsets
    private void OnHeadsetChanged(object sender, List<Headset> newHeadsets)
    {
        print("---------------SUCCESS---------------");
        newInfo = true;
        headsets = newHeadsets;
    }

    private void ShowHeadsets()
    {
        if (connected)
            return;
        // destroy all headsets in list
        foreach (Transform child in headsetList)
            Destroy(child.gameObject);

        if (headsets.Count == 0)
        {
            print("No Headsets detected");
            return;
        }

        // loop through detected headsets and add them to the UI list
        foreach (var item in headsets)
        {
            // this should never happen, but is included in the example so it is here just in case
            if (item == null)
            {
                print("headset list item value was null");
                continue;
            }

            ConnectableHeadset newHeadset = Instantiate(headsetItemPrefab, headsetList).GetComponent<ConnectableHeadset>();
            newHeadset.Init(item);
        }
    }

    private void OnConnect(object sender, HeadsetConnectEventArgs args)
    {
        print("Headset connected!");
        //training.QueryProfile();
        connected = true;
    }

    public void LoadProfile(string profileName)
    {
        //training.LoadProfile(profileName);

    }

    private void OnProfileQuery(object sender, List<string> args)
    {

        //print("------------------------------------------------");
        //foreach (var i in training.ProfileLists)
        //    print(i);
        //print("------------------------------------------------");
    }

    public void TriggerHeadsetQuery()
    {
        headsetFinder.TriggerQuery();
        //CortexClient.Instance.QueryHeadsets("");
        //if(socket == null)
        //{
        //    socket = new WebSocket(Config.AppUrl);
        //    socket.Open();
        //    socket.MessageReceived += OnSocketMessageRecieved;
        //}
        //socket.Send();
    }

    //private void OnSocketMessageRecieved(object sender, MessageReceivedEventArgs args)
    //{
    //    string message = args.Message;
    //    print($"message from custom socket: {message}");
    //    JObject response = JObject.Parse(message);
    //}

    public void DebugTest()
    {
        //CortexClient.Instance.Se();
        data.StartSession("EPOCX-03030281");
    }
}
