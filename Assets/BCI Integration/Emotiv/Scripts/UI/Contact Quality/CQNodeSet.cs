using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System;
using TMPro;

public class CQNodeSet : MonoBehaviour
{
    protected CQNode[] nodes;


    public virtual void Init(Color[] nodeColours)
    {
        List<CQNode> nodeList = new List<CQNode>();
        foreach (Transform child in transform)
            nodeList.Add(child.GetComponent<CQNode>());
        nodes = nodeList.ToArray();

        foreach (CQNode node in nodes)
            node.Init(nodeColours);
    }

    public virtual void OnCQUpdate(DeviceInfo data)
    {
        foreach (CQNode node in nodes)
            node.UpdateQuality(data);
    }
}
