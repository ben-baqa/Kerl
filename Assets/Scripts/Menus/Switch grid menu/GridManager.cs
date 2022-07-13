using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class GridManager : MonoBehaviour
{
    public float selectionPeriod;

    public List<LockedNode> nodeInfo;

    public Sprite borderSprite;
    public NodePlacementSettings placement;

    //bool[] Locked;
    //List<NodeElement> NodeInfo;
    //public List<Color> Colors;

    //public float nodeSize;
    //public float nodeImageSize;
    //public float nodeSelectRatio;

    //public float selectorSize;

    //public float spacing;


    public UnityEvent<int> OnNodeSelected;

    SelectorDrawer selectorDrawer;
    List<Node> nodes;

    GridCursor cursor;

    int columns;
    int rows;

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        if (nodeInfo == null)
            return;

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(placement.GetNodePositionDebug(1, nodeInfo.Count),
            new Vector2(placement.selectorSize + placement.spacing * (placement.columns - 1), placement.selectorSize));

        Gizmos.color = new Color(1, 1, 1, 0.5f);
        for(int i = 0; i < nodeInfo.Count; i++)
        {
            Vector3 pos = placement.GetNodePositionDebug(i, nodeInfo.Count);
            Gizmos.DrawCube(pos, Vector3.one * placement.size);
            Gizmos.DrawCube(pos, Vector3.one * placement.selectedImageSize);
        }

    }

    private void Start()
    {
        columns = placement.columns;
        rows = placement.rows = (int)Mathf.Ceil((float)nodeInfo.Count / placement.columns);
        print(placement.rows);

        bool[] locked = new bool[nodeInfo.Count];
        for (int i = 0; i < locked.Length; i++)
            locked[i] = nodeInfo[i].locked;

        cursor = gameObject.AddComponent<GridCursor>();
        cursor.Init(columns, rows, locked, selectionPeriod, UpdateCursor, UpdateSelection);
        //cursor.columns = columns;
        //cursor.rows = rows;
        //cursor.locked = locked;
        //cursor.period = selectionPeriod;
        //cursor.CursorUpdate.AddListener(UpdateCursor);
        //cursor.SelectionUpdate.AddListener(UpdateSelection);

        GameObject selectorDrawerObject = new GameObject("Selector Drawer", typeof(RectTransform));
        selectorDrawer = selectorDrawerObject.AddComponent<SelectorDrawer>();
        selectorDrawer.Init(borderSprite, placement, transform);
        //selectorDrawer.borderSprite = BorderSprite;
        //selectorDrawer.columns = columns;
        //selectorDrawer.rows = rows;
        //selectorDrawer.size = selectorSize;
        //selectorDrawer.spacing = spacing;
        //selectorDrawer.colors = Colors;

        nodes = new List<Node>();
        for (int i = 0; i < nodeInfo.Count; i++)
        {
            GameObject newNodeObject = new GameObject("Node", typeof(RectTransform));
            newNodeObject.transform.SetParent(transform, false);
            newNodeObject.transform.localPosition = placement.GetNodePosition(i);

            Node newNode = newNodeObject.AddComponent<Node>();
            newNode.Init(borderSprite, nodeInfo[i].Image, placement, locked[i]);
            nodes.Add(newNode);
            //newNode.BorderSprite = borderSprite;
            //newNode.ImageSprite = NodeInfo[i].Image;
            //newNode.Size = nodeSize;
            //newNode.ImageSize = nodeImageSize;
            //newNode.SelectedRatio = nodeSelectRatio;
            //newNode.Disabled = Locked[i];
        }
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        int current = cursor.GetCursor();
        int nodeCount;
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].SetSelected(false);
        }
        if (current < rows)
        {
            nodeCount = Mathf.Min(columns, nodes.Count - current * columns);
        }
        else
        {
            nodeCount = Mathf.Min(rows, nodes.Count / (current + 1 - rows));
        }
        for (int i = 0; i < nodeCount; i++) {
            int currentNode;
            if (current < rows)
            {
                currentNode = current * columns + i;
            }
            else
            {
                currentNode = i * columns + current - rows;
            }
            nodes[currentNode].SetSelected(true);
        }
    }

    private void UpdateSelection()
    {
        List<int> selectedNodesSet = new List<int>(); 
        int[] selectedNodes = new int[InputProxy.playerCount];
        for (int i = 0; i < InputProxy.playerCount; i++) {
            int node = cursor.GetSelectedNode(i);
            if (node < 0)
            {
                int row = cursor.GetSelectedRow(i);
                int column = cursor.GetSelectedColumn(i);
                selectorDrawer.AddSelection(i, true, row);
                selectorDrawer.AddSelection(i, false, column);
            }
            else {
                selectorDrawer.AddSelection(i, true, -1);
                selectorDrawer.AddSelection(i, false, -1);
                if (!selectedNodesSet.Contains(node)) {
                    selectedNodesSet.Add(node);
                }
                OnNodeSelected.Invoke(i);
            }
            selectedNodes[i] = node;
        }
        foreach (Node node in nodes) {
            node.ChangeColor(new Color[] { });
        }
        for (int i = 0; i < selectedNodesSet.Count; i++) {
            List<Color> nodeColors = new List<Color>();
            for (int j = 0; j < InputProxy.playerCount; j++) {
                if (selectedNodes[j] == selectedNodesSet[i]) {
                    nodeColors.Add(MenuSelections.GetColor(j));
                }
            }
            nodes[selectedNodesSet[i]].ChangeColor(nodeColors.ToArray());
        }
        selectorDrawer.DrawSelectors();
    }

    public string GetStringSelection(int color)
    {
        int selected = cursor.GetSelectedNode(color);
        return nodeInfo[selected].StringPayload;
    }

    public GameObject GetPrefabSelection(int color)
    {
        int selected = cursor.GetSelectedNode(color);
        return nodeInfo[selected].PrefabPayload;
    }

    
}
[System.Serializable]
public class NodePlacementSettings
{
    [Min(1)]
    public int columns;
    [HideInInspector]
    public int rows;

    public float size;
    public float selectedImageSize;
    public float highlightedSizeRatio;
    public float selectorSize;
    public float spacing;

    public Vector2 Offset
    {
        get
        {
            if (_offset != Vector2.zero)
                return _offset;
            _offset = new Vector2(spacing * (1 - columns) / 2f, -spacing * (1 - rows) / 2f);
            return _offset;
        }
    }
    Vector2 _offset = Vector2.zero;

    public Vector2 GetNodePosition(int index)
    {
        Vector2 pos = Vector2.zero;

        pos.x = spacing * (index % columns);
        pos.y = -spacing * (index / columns);
        return pos + Offset;
    }

    public Vector2 GetNodePositionDebug(int index, int total)
    {
        int rows = (int)Mathf.Ceil((float)total / columns);
        Vector2 pos = Vector2.zero;

        pos.x = spacing * (index % columns);
        pos.y = -spacing * (index / columns);
        return pos + new Vector2(spacing * (1 - columns) / 2f, -spacing * (1 - rows) / 2f);
    }
}

[System.Serializable]
public class LockedNode
{
    public NodeElement node;
    public bool locked;

    public string ItemName => node.ItemName;
    public Sprite Image => node.Image;
    public NodeElement.PayloadType Type => node.Type;
    public GameObject PrefabPayload => node.PrefabPayload;
    public string StringPayload => node.StringPayload;
}
