using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactQualityDisplay : MonoBehaviour
{
    public NodeSet[] displays;

    public string headsetID;


    public void Activate()
    {
        gameObject.SetActive(true);

        foreach (var i in displays)
            if (headsetID.StartsWith(i.type))
                i.nodeSet.Init(headsetID);
            else
                i.nodeSet.gameObject.SetActive(false);
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
