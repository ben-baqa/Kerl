using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EmotivUnityPlugin;

public class CQNode : MonoBehaviour
{
    public string channelID;
    Channel_t channel;
    Image display;

    Color[] colours;

    private void Start()
    {
        display = GetComponent<Image>();
        channel = ChannelStringList.StringToChannel(channelID);
    }

    public void SetColours(Color[] ar)
    {
        colours = ar;
    }

    public void UpdateQuality(DevData data)
    {
        int val;
        if (data.contactQuality.ContainsKey(channel))
            val = (int)data.contactQuality[channel];
        else
            val = (int)(data.cqOverall / 20);

        if (val < colours.Length)
            display.color = colours[val];
    }
}
