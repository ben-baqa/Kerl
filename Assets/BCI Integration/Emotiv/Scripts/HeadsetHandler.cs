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


    void Start()
    {
        data.onHeadsetChange += OnHeadsetChanged;
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

            print("Headset Detected!");
            print(item.Value);
        }
    }
}
