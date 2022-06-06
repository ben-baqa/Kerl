using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridManager : MonoBehaviour
{
    public int columns;
    public int rows;
    public bool[] locked;
    public List<NodeElement> nodeInfo;
    public Sprite borderSprite;

    public float nodeSize;
    public float nodeImageSize;
    public float nodeSelectRatio;

    public float selectorSize;

    public float spacing;

    public float delayTime;

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

        nodes = new List<Node>();
        for (int i = 0; i < nodeInfo.Count; i++)
        {
            GameObject newNodeObject = new GameObject("Node", typeof(RectTransform), typeof(Node));
            newNodeObject.GetComponent<RectTransform>().SetParent(transform);
            newNodeObject.GetComponent<RectTransform>().localPosition = new Vector2(spacing * (i % columns), -spacing * (i / columns));
            Node newNode = newNodeObject.GetComponent<Node>();
            newNode.borderSprite = borderSprite;
            newNode.imageSprite = nodeInfo[i].image;
            newNode.size = nodeSize;
            newNode.imageSize = nodeImageSize;
            nodes.Add(newNode);
        }
    }

    private void UpdateCursor()
    {

    }

    private void UpdateSelection()
    {

    }

    public string GetStringSelection(int color)
    {
        int selected = cursor.GetSelectedNode(color);
        return nodeInfo[selected].stringPayload;
    }

    public GameObject GetPrefabSelection(int color)
    {
        int selected = cursor.GetSelectedNode(color);
        return nodeInfo[selected].prefabPayload;
    }
}