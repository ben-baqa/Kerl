using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System;

public class HeadsetConnectionHandler : MonoBehaviour
{
    DataProcessing data = DataProcessing.Instance;
    BCITraining training;

    [SerializeField] GameObject headsetItemPrefab;
    [SerializeField] Transform headsetList;

    MentalCommand previousCommand;
    bool connected = false;

    void Start()
    {
        training = new BCITraining();
        training.Init();
        data.onHeadsetChange += OnHeadsetChanged;
        data.HeadsetConnected += OnConnect;

        TrainingHandler.Instance.QueryProfileOK += OnProfileQuery;
    }

    private void Update()
    {
        var mentalCommandData = DataStreamManager.Instance.GetMentalCommands();
        if (mentalCommandData == null)
            return;
        foreach (var command in mentalCommandData)
        {
            if (command != previousCommand)
                print(command);

            previousCommand = command;

            //displayBar.displayText = command.action;
            //displayBar.progress = command.power;
        }
    }

    private void OnDestroy()
    {
        data.onHeadsetChange -= OnHeadsetChanged;
        data.HeadsetConnected -= OnConnect;
    }

    // called by the event system when there is a change in the list of available headsets
    private void OnHeadsetChanged(object sender, EventArgs args)
    {
        if (connected)
            return;
        // destroy all headsets in list
        foreach (Transform child in headsetList)
            Destroy(child.gameObject);

        if (data.GetHeadsetList().Count == 0)
        {
            print("No Headsets detected");
            return;
        }

        // loop through detected headsets and add them to the UI list
        foreach (var item in data.GetHeadsetList())
        {
            // this should never happen, but is included in the example so it is here just in case
            if (item.Value == null)
            {
                print("headset list item value was null");
                continue;
            }

            ConnectableHeadset newHeadset = Instantiate(headsetItemPrefab, headsetList).GetComponent<ConnectableHeadset>();
            newHeadset.Init(item.Value);
        }
    }

    private void OnConnect(object sender, string args)
    {
        training.QueryProfile();
        connected = true;
    }

    public void LoadProfile(string profileName)
    {
        training.LoadProfile(profileName);

    }

    private void OnProfileQuery(object sender, List<string> args)
    {

        //print("------------------------------------------------");
        //foreach (var i in training.ProfileLists)
        //    print(i);
        //print("------------------------------------------------");
    }
}
