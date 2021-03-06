using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmotivUnityPlugin;

public class QualityHUD : MonoBehaviour
{
    public Image batteryIndicator;
    public Sprite[] batterySprites;

    public TextMeshProUGUI contactQualityText;
    public Color[] colours;
    public Image contactQualityBackground;
    public Sprite[] backgroundSprites;

    string headsetID;

    private void Start()
    {
        Cortex.DataStreamStarted += Init;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (!string.IsNullOrEmpty(headsetID))
            Cortex.SubscribeDeviceInfo(headsetID, OnDeviceInfoReceived);
    }
    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(headsetID))
            Cortex.UnsubscribeDeviceInfo(headsetID, OnDeviceInfoReceived);
    }

    public void Init(string headset)
    {
        if (!string.IsNullOrEmpty(headsetID))
            Cortex.UnsubscribeDeviceInfo(headset, OnDeviceInfoReceived);

        headsetID = headset;
        Cortex.SubscribeDeviceInfo(headset, OnDeviceInfoReceived);
        gameObject.SetActive(true);
    }

    void OnDeviceInfoReceived(DeviceInfo data)
    {
        double quality = data.cqOverall;
        contactQualityText.text = $"{quality}";

        contactQualityText.color = colours[PercentToIndex(quality, colours.Length)];
        contactQualityBackground.sprite =
        backgroundSprites[PercentToIndex(quality, backgroundSprites.Length)];

        batteryIndicator.sprite = batterySprites[PercentToIndex(data.battery, batterySprites.Length)];
    }

    int PercentToIndex(double percentage, int options)
    {
        return (int)Mathf.Clamp((float)percentage * options / 101 , 0, options - 1);
    }
}
