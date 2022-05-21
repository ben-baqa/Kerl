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
    //DevData data

    float batteryUpdateTimer = 1;
    const float BATTERY_UPDATE_INTERVAL = 2f;

    
    void Start()
    {
        //DataProcessing.Instance.onContactQualityUpdated += OnCQUpdate;
    }
    private void OnDestroy()
    {
        DataStreamManager.Instance.Unsubscribe<DevInfo>(headsetID, OnCQUpdate);
        //DataProcessing.Instance.onContactQualityUpdated -= OnCQUpdate;
    }

    public void Init(string headset)
    {
        headsetID = headset;
        DataStreamManager.Instance.SubscribeTo<DevInfo>(headset, OnCQUpdate);
    }

    void Update()
    {
        batteryUpdateTimer += Time.deltaTime;
        if (batteryUpdateTimer < BATTERY_UPDATE_INTERVAL)
            return;
        batteryUpdateTimer -= BATTERY_UPDATE_INTERVAL;

        //double batteryLevel = DataStreamManager.Instance.Battery()
        //    / DataStreamManager.Instance.BatteryMax();
        //print($"Battery: {DataStreamManager.Instance.Battery()}," +
        //    $"Battery Max: {DataStreamManager.Instance.BatteryMax()}");
        //batteryIndicator.sprite = batterySprites[Index(batteryLevel * 100, batterySprites.Length)];
    }

    void OnCQUpdate(DevInfo data)
    {

    }

    //void OnCQUpdate(object sender, System.EventArgs args)
    //{
    //    double quality = DataStreamManager.Instance.GetContactQuality(Channel_t.CHAN_CQ_OVERALL);// DataProcessing.Instance.GetCQOverAll();
    //    contactQualityText.text = $"{(int)quality}";

    //    contactQualityText.color = colours[Index(quality, colours.Length)];
    //    contactQualityBackground.sprite =
    //        backgroundSprites[Index(quality,backgroundSprites.Length)];

    //    double batteryLevel = DataStreamManager.Instance.GetContactQuality(Channel_t.CHAN_BATTERY_PERCENT);
    //    //print($"Battery: {batteryLevel}");
    //    batteryIndicator.sprite = batterySprites[Index(batteryLevel, batterySprites.Length)];
    //}

    int Index(double percentage, int options)
    {
        return (int)Mathf.Clamp((float)percentage * options / 101 , 0, options - 1);
    }
}
