using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using TMPro;

public class VariableCQNodeSet : CQNodeSet
{
    public GameObject nodePrefab;
    public Transform nodeList;

    public override void Start()
    {
        nodes = new CQNode[0];
    }

    public override void OnCQUpdate(DevData data)
    {
        int expectedNodeCount = data.cqHeaders.Count;
        if (nodes.Length != expectedNodeCount)
        {
            Init(data, expectedNodeCount);
        }

        foreach (CQNode node in nodes)
            node.UpdateQuality(data);
    }

    void Init(DevData data, int count)
    {
        print("Initializing variable contact quality display");
        nodes = new CQNode[count];
        nodes[0] = MakeNode("Ref");
        for (int i = 1; i < count; i++)
        {
            nodes[i] = MakeNode(data.cqHeaders[i - 1]);
        }

        foreach (CQNode node in nodes)
            node.SetColours(nodeColours);
    }

    CQNode MakeNode(string header)
    {
        CQNode node = Instantiate(nodePrefab, nodeList).GetComponentInChildren<CQNode>();
        node.channelID = header;
        node.GetComponentInChildren<TextMeshProUGUI>().text = header;
        return node;
    }
}
