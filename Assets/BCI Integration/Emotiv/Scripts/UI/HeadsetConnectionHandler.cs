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

    [SerializeField] GameObject headsetItemPrefab;
    [SerializeField] Transform headsetList;

    bool connected = false;

    List<Headset> headsets;
    bool newInfo = false;

    public ContactQualityDisplay contactQualityDisplay;

    void Start()
    {
        data.HeadsetConnected += OnConnect;

        data.QueryHeadsetOK += OnHeadsetChanged;
        TriggerHeadsetQuery();
    }
    private void OnDestroy()
    {
        data.HeadsetConnected -= OnConnect;
        data.QueryHeadsetOK += OnHeadsetChanged;
    }

    private void Update()
    {
        if(connected)
        {
            contactQualityDisplay.Activate();
            gameObject.SetActive(false);
        }
        if (newInfo)
        {
            newInfo = false;
            ShowHeadsets();
        }
    }

    // called by the event system when there is a change in the list of available headsets
    private void OnHeadsetChanged(object sender, List<Headset> newHeadsets)
    {
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

    private void OnConnect(object sender, string headsetID)
    {
        print("=================== Headset connected!");
        //training.QueryProfile();
        connected = true;
        contactQualityDisplay.AssignHeadset(headsetID);
    }

    public void TriggerHeadsetQuery()
    {
        headsetFinder.TriggerQuery();
    }
}
