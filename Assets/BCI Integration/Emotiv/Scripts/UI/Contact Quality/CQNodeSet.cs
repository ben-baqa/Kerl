using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System;

public class CQNodeSet : MonoBehaviour
{
    public Color[] nodeColours;

    CQNode[] nodes;

    float cqUpdateTimer = 0;
    const float CQ_UPDATE_INTERVAL = 0.5f;

    void Start()
    {
        List<CQNode> nodeList = new List<CQNode>();
        foreach (Transform child in transform)
            nodeList.Add(child.GetComponent<CQNode>());
        nodes = nodeList.ToArray();

        foreach (CQNode node in nodes)
            node.SetColours(nodeColours);

        //DataProcessing.Instance.onContactQualityUpdated += OnCQUpdate;
    }
    private void OnDestroy()
    {
        //DataProcessing.Instance.onContactQualityUpdated -= OnCQUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        cqUpdateTimer += Time.deltaTime;
        if (cqUpdateTimer < CQ_UPDATE_INTERVAL)
            return;

        cqUpdateTimer -= CQ_UPDATE_INTERVAL;

        //DataProcessing.Instance.updateContactQuality();
        foreach (CQNode node in nodes)
            node.UpdateQuality();
    }

    void OnCQUpdate(object sender, EventArgs args)
    {
        foreach (CQNode node in nodes)
            node.UpdateQuality();
    }
}
