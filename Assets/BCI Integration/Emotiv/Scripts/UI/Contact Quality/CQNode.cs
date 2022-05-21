using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EmotivUnityPlugin;
using TMPro;

public class CQNode : MonoBehaviour
{
    public string channelID;
    Channel_t channel;
    Image display;

    Color[] colours;

    public void Init(Color[] ar)
    {
        colours = ar;

        display = GetComponent<Image>();
        channel = ChannelStringList.StringToChannel(channelID);

        TextMeshProUGUI label = GetComponentInChildren<TextMeshProUGUI>();
        if (label)
            label.text = channelID;
    }

    public void UpdateQuality(DevInfo data)
    {
        int val;
        if (data.contactQuality.ContainsKey(channel))
            val = (int)data.contactQuality[channel];
        else
            val = GetReferenceQuality(data);

        if (val < colours.Length)
            display.color = colours[val];
    }

    int GetReferenceQuality(DevInfo data)
    {
        foreach (var channel in data.contactQuality.Values)
            if (channel > (int)ContactQualityValue.VERY_BAD)
                return (int)ContactQualityValue.GOOD;
        return (int)ContactQualityValue.VERY_BAD;
    }
}
