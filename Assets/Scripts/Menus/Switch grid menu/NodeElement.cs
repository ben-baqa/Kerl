using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNodeElement", menuName = "ScriptableObjects/NodeElement", order = 1)]
public class NodeElement : ScriptableObject
{
    public enum PayloadType
    {
        Prefab,
        String
    }

    public string itemName;
    public Sprite image;
    public PayloadType type;

    public GameObject prefabPayload;
    public string stringPayload;
}