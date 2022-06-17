using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class GridManager : MonoBehaviour
{
    public int Columns;
    public int Rows;
    public bool[] Locked;
    public List<NodeElement> NodeInfo;
    public List<Color> Colors;
    public Sprite BorderSprite;

    public float NodeSize;
    public float NodeImageSize;
    public float NodeSelectRatio;

    public float SelectorSize;

    public float Spacing;

    public float DelayTime;

    public UnityEvent<int> OnNodeSelected;

    private SelectorDrawer _selectorDrawer;
    private List<Node> _nodes;

    private GridCursor _cursor;

    private void Start()
    {
        _cursor = gameObject.AddComponent<GridCursor>();
        _cursor.Columns = Columns;
        _cursor.Rows = Rows;
        _cursor.Nodes = Locked;
        _cursor.DelayTime = DelayTime;
        _cursor.CursorUpdate.AddListener(UpdateCursor);
        _cursor.SelectionUpdate.AddListener(UpdateSelection);

        GameObject selectorDrawerObject = new GameObject("Selector Drawer", typeof(RectTransform), typeof(SelectorDrawer));
        selectorDrawerObject.GetComponent<RectTransform>().SetParent(transform);
        selectorDrawerObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
        _selectorDrawer = selectorDrawerObject.GetComponent<SelectorDrawer>();
        _selectorDrawer.BorderSprite = BorderSprite;
        _selectorDrawer.Columns = Columns;
        _selectorDrawer.Rows = Rows;
        _selectorDrawer.Size = SelectorSize;
        _selectorDrawer.Spacing = Spacing;
        _selectorDrawer.Colors = Colors;

        _nodes = new List<Node>();
        for (int i = 0; i < NodeInfo.Count; i++)
        {
            GameObject newNodeObject = new GameObject("Node", typeof(RectTransform), typeof(Node));
            newNodeObject.GetComponent<RectTransform>().SetParent(transform);
            newNodeObject.GetComponent<RectTransform>().localPosition = new Vector2(Spacing * (i % Columns), -Spacing * (i / Columns));
            Node newNode = newNodeObject.GetComponent<Node>();
            newNode.BorderSprite = BorderSprite;
            newNode.ImageSprite = NodeInfo[i].Image;
            newNode.Size = NodeSize;
            newNode.ImageSize = NodeImageSize;
            newNode.SelectedRatio = NodeSelectRatio;
            newNode.Disabled = Locked[i];
            _nodes.Add(newNode);
        }
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        int current = _cursor.GetCursor();
        int nodeCount;
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].SetSelected(false);
        }
        if (current < Rows)
        {
            nodeCount = Mathf.Min(Columns, _nodes.Count - current * Columns);
        }
        else
        {
            nodeCount = Mathf.Min(Rows, _nodes.Count / (current + 1 - Rows));
        }
        for (int i = 0; i < nodeCount; i++) {
            int currentNode;
            if (current < Rows)
            {
                currentNode = current * Columns + i;
            }
            else
            {
                currentNode = i * Columns + current - Rows;
            }
            _nodes[currentNode].SetSelected(true);
        }
    }

    private void UpdateSelection()
    {
        List<int> selectedNodesSet = new List<int>(); 
        int[] selectedNodes = new int[InputProxy.playerCount];
        for (int i = 0; i < InputProxy.playerCount; i++) {
            int node = _cursor.GetSelectedNode(i);
            if (node < 0)
            {
                int row = _cursor.GetSelectedRow(i);
                int column = _cursor.GetSelectedColumn(i);
                _selectorDrawer.AddSelection(i, true, row);
                _selectorDrawer.AddSelection(i, false, column);
            }
            else {
                _selectorDrawer.AddSelection(i, true, -1);
                _selectorDrawer.AddSelection(i, false, -1);
                if (!selectedNodesSet.Contains(node)) {
                    selectedNodesSet.Add(node);
                }
                OnNodeSelected.Invoke(i);
            }
            selectedNodes[i] = node;
        }
        foreach (Node node in _nodes) {
            node.ChangeColor(new Color[] { });
        }
        for (int i = 0; i < selectedNodesSet.Count; i++) {
            List<Color> nodeColors = new List<Color>();
            for (int j = 0; j < InputProxy.playerCount; j++) {
                if (selectedNodes[j] == selectedNodesSet[i]) {
                    nodeColors.Add(Colors[j]);
                }
            }
            _nodes[selectedNodesSet[i]].ChangeColor(nodeColors.ToArray());
        }
        _selectorDrawer.DrawSelectors();
    }

    public string GetStringSelection(int color)
    {
        int selected = _cursor.GetSelectedNode(color);
        return NodeInfo[selected].StringPayload;
    }

    public GameObject GetPrefabSelection(int color)
    {
        int selected = _cursor.GetSelectedNode(color);
        return NodeInfo[selected].PrefabPayload;
    }
}
