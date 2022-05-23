using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System;
using WebSocket4Net;
using Newtonsoft.Json.Linq;

public class HeadsetConnectionHandler : MonoBehaviour
{
    [SerializeField] GameObject headsetItemPrefab;
    [SerializeField] Transform headsetList;

    public ContactQualityDisplay contactQualityDisplay;

    void OnEnable()
    {
        Cortex.QueryHeadsetOK += OnHeadsetChanged;
        Cortex.HeadsetConnected += OnConnect;

        TriggerHeadsetQuery();
    }
    private void OnDisable()
    {
        Cortex.QueryHeadsetOK -= OnHeadsetChanged;
        Cortex.HeadsetConnected -= OnConnect;
    }

    // called by the event system when there is a change in the list of available headsets
    private void OnHeadsetChanged(List<Headset> headsets)
    {
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

    private void OnConnect(string headsetID)
    {
        print("=================== Headset connected!");

        contactQualityDisplay.AssignHeadset(headsetID);
        contactQualityDisplay.Activate();
        gameObject.SetActive(false);
    }

    public void TriggerHeadsetQuery()
    {
        Cortex.QueryHeadsets();
    }
}
