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
        //try
        //{
        //    channel = (Channel_t)System.Enum.Parse(typeof(Channel_t), $"CHAN_{channelID}");
        //}
        //catch(System.ArgumentException e)
        //{
        //    print($"Invalid channelID on Contact Quality Node display: {channelID}");
        //}
    }

    public void SetColours(Color[] ar)
    {
        colours = ar;
    }

    public void UpdateQuality(/*ContactQualityValue[] qualitySet*/)
    {
        //int val = (int)DataStreamManager.Instance.GetContactQuality(channel);
        ////int val = (int)qualitySet[(int)channel];
        //print($"Channel: {channel}, quality: {val}");

        //if (val < colours.Length)
        //    display.color = colours[val];
    }
}
