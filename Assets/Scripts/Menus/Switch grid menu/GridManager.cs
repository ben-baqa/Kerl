using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class GridManager : MonoBehaviour
{
    public int columns;
    public int rows;
    public bool[] locked;
    public List<NodeElement> nodeInfo;
    public List<Color> colors;
    public Sprite borderSprite;

    public float nodeSize;
    public float nodeImageSize;
    public float nodeSelectRatio;

    public float selectorSize;

    public float spacing;

    public float delayTime;

    public UnityEvent<int> OnNodeSelected;

    private SelectorDrawer selectorDrawer;
    private List<Node> nodes;

    private GridCursor cursor;

    private void Start()
    {
        cursor = gameObject.AddComponent<GridCursor>();
        cursor.columns = columns;
        cursor.rows = rows;
        cursor.nodes = locked;
        cursor.delayTime = delayTime;
        cursor.cursorUpdate.AddListener(UpdateCursor);
        cursor.selectionUpdate.AddListener(UpdateSelection);

        GameObject selectorDrawerObject = new GameObject("Selector Drawer", typeof(RectTransform), typeof(SelectorDrawer));
        selectorDrawerObject.GetComponent<RectTransform>().SetParent(transform);
        selectorDrawerObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
        selectorDrawer = selectorDrawerObject.GetComponent<SelectorDrawer>();
        selectorDrawer.borderSprite = borderSprite;
        selectorDrawer.columns = columns;
        selectorDrawer.rows = rows;
        selectorDrawer.size = selectorSize;
        selectorDrawer.spacing = spacing;
        selectorDrawer.colors = colors;

        nodes = new List<Node>();
        for (int i = 0; i < nodeInfo.Count; i++)
        {
            GameObject newNodeObject = new GameObject("Node", typeof(RectTransform), typeof(Node));
            newNodeObject.GetComponent<RectTransform>().SetParent(transform);
            newNodeObject.GetComponent<RectTransform>().localPosition = new Vector2(spacing * (i % columns), -spacing * (i / columns));
            Node newNode = newNodeObject.GetComponent<Node>();
            newNode.borderSprite = borderSprite;
            newNode.imageSprite = nodeInfo[i].Image;
            newNode.size = nodeSize;
            newNode.imageSize = nodeImageSize;
            newNode.selectedRatio = nodeSelectRatio;
            newNode.disabled = locked[i];
            nodes.Add(newNode);
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
                    nodeColors.Add(colors[j]);
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
