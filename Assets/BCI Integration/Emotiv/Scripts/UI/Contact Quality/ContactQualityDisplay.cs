using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using TMPro;
using Text = TMPro.TextMeshProUGUI;

public class ContactQualityDisplay : MonoBehaviour
{
    DataStreamManager dataStreamManager = DataStreamManager.Instance;

    public NodeSet[] displays;
    public CQNodeSet fallback;
    public Text contactQualityPercentage;

    CQNodeSet activeDisplay = null;
    DevData devData;
    string headsetID;
    bool newData;

    private void Update()
    {
        if (newData)
        {
            newData = false;
            activeDisplay.OnCQUpdate(devData);
            contactQualityPercentage.text = $"{devData.cqOverall}%";
        }
    }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(headsetID))
            dataStreamManager.SubscribeTo<DevData>(headsetID, OnDevDataRecieved);
    }
    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(headsetID))
            dataStreamManager.Unsubscribe<DevData>(headsetID, OnDevDataRecieved);
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        foreach (var i in displays)
            if (headsetID.StartsWith(i.type))
            {
                activeDisplay = i.nodeSet;
                activeDisplay.gameObject.SetActive(true);
            }
            else
                i.nodeSet.gameObject.SetActive(false);
        if(activeDisplay == null)
        {
            fallback.gameObject.SetActive(true);
        }
    }

    public void AssignHeadset(string id)
    {
        if (!string.IsNullOrEmpty(headsetID))
            dataStreamManager.Unsubscribe<DevData>(headsetID, OnDevDataRecieved);
        headsetID = id;
        dataStreamManager.SubscribeTo<DevData>(headsetID, OnDevDataRecieved);
    }

    void OnDevDataRecieved(DevData data)
    {
        newData = true;
        devData = data;
    }

    public void Continue()
    {
        // move on to choosing or training a profile
    }

    [System.Serializable]
    public struct NodeSet
    {
        public string type;
        public CQNodeSet nodeSet;
    }
}
