using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System;

public class CQNodeSet : MonoBehaviour
{
    public Color[] nodeColours;

    CQNode[] nodes;

    //DevData devData;
    //bool newData;

    void Start()
    {
        List<CQNode> nodeList = new List<CQNode>();
        foreach (Transform child in transform)
            nodeList.Add(child.GetComponent<CQNode>());
        nodes = nodeList.ToArray();

        foreach (CQNode node in nodes)
            node.SetColours(nodeColours);
    }

    //void Update()
    //{
    //    if (newData)
    //    {
    //        newData = false;
    //    }
    //}

    public void OnCQUpdate(DevData data)
    {
        foreach (CQNode node in nodes)
            node.UpdateQuality(data);
        //devData = data;
        //newData = true;
    }

    //public void Init(string headsetID)
    //{
    //    Debug.Log($"Contact quality Node set successfully activated for headset: {headsetID}");
    //    gameObject.SetActive(true);
    //    DataStreamManager.Instance[headsetID].DevDataReceived += OnCQUpdate;
    //}
}
