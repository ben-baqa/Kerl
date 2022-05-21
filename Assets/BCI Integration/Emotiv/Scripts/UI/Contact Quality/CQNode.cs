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
            val = GetReferenceQuality(data);

        if (val < colours.Length)
            display.color = colours[val];
    }

    int GetReferenceQuality(DevData data)
    {
        foreach (var channel in data.contactQuality.Values)
            if (channel > (int)ContactQualityValue.VERY_BAD)
                return (int)ContactQualityValue.GOOD;
        return (int)ContactQualityValue.VERY_BAD;
    }
}
