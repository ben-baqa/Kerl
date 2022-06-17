using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNodeElement", menuName = "ScriptableObjects/NodeElement", order = 1)]
public class NodeElement : ScriptableObject
{
    public enum PayloadType { 
        Prefab,
        String
    }

    public string ItemName;
    public Sprite Image;
    public PayloadType Type;

    public GameObject PrefabPayload;
    public string StringPayload;
}
