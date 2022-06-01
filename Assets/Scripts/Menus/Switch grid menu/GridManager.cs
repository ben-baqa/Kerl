using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GridManager : MonoBehaviour
{
    public Sprite sprite;
    public float nodeSize;
    public float selectorSize;
    public float spacing;

    private Color[] colors = new Color[] { Color.black, Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow, Color.grey };
    private GlobalCursor gc;
    private List<GameObject> nodes;
    private List<GameObject> selectors;

    void Start()
    {
        gc = gameObject.GetComponent<GlobalCursor>();
        nodes = new List<GameObject>();
        selectors = new List<GameObject>();

        for (int j = 0; j < InputProxy.playerCount; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject newSelector = new GameObject("Selector", typeof(Image));
                newSelector.GetComponent<RectTransform>().SetParent(transform);
                newSelector.GetComponent<Image>().sprite = sprite;
                newSelector.GetComponent<Image>().color = colors[j];
                newSelector.SetActive(false);
                selectors.Add(newSelector);
            }
            selectors[j * 3 + 0].GetComponent<RectTransform>().sizeDelta = new Vector3(selectorSize + spacing * (gc.columns - 1), selectorSize, 0);
            selectors[j * 3 + 1].GetComponent<RectTransform>().sizeDelta = new Vector3(selectorSize, selectorSize + spacing * (gc.rows - 1), 0);
            selectors[j * 3 + 2].GetComponent<RectTransform>().sizeDelta = new Vector3(selectorSize, selectorSize, 0);
        }

        for (int i = 0; i < gc.rows; i++)
        {
            for (int j = 0; j < gc.columns; j++)
            {
                int currentNode = i * gc.columns + j;
                if (currentNode >= gc.nodes.Length)
                {
                    break;
                }
                GameObject newNode = new GameObject("Node", typeof(Image));
                newNode.GetComponent<RectTransform>().SetParent(transform);
                newNode.GetComponent<RectTransform>().sizeDelta = new Vector3(nodeSize, nodeSize, 0);
                newNode.GetComponent<RectTransform>().localPosition = new Vector3(spacing * j, -spacing * i, 0);
                newNode.GetComponent<Image>().sprite = sprite;
                if (gc.nodes[currentNode])
                {
                    newNode.GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    newNode.GetComponent<Image>().color = Color.white;
                }
                nodes.Add(newNode);
            }
        }

        CursorUpdate();
        gc.cursorUpdate.AddListener(CursorUpdate);
        gc.selectionUpdate.AddListener(SelectionUpdate);
    }

    void CursorUpdate()
    {
        foreach (GameObject node in nodes)
        {
            node.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0);
        }
        int cursor = gc.GetCursor();
        int nodeCount;
        if (cursor < gc.rows)
        {
            nodeCount = Mathf.Min(gc.columns, gc.nodes.Length - cursor * gc.columns);
        }
        else
        {
            nodeCount = Mathf.Min(gc.rows, (gc.nodes.Length) / (cursor + 1 - gc.rows));
        }
        for (int i = 0; i < nodeCount; i++)
        {
            int currentNode;
            if (cursor < gc.rows)
            {
                currentNode = cursor * gc.columns + i;
            }
            else
            {
                currentNode = i * gc.columns + cursor - gc.rows;
            }
            nodes[currentNode].GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 0);
        }
    }

    void SelectionUpdate()
    {
        for (int i = 0; i < InputProxy.playerCount; i++)
        {
            int currentNode = gc.GetSelectedNode(i);
            if (currentNode < 0)
            {
                int currentRow = gc.GetSelectedRow(i);
                int currentColumn = gc.GetSelectedColumn(i);
                selectors[i * 3 + 0].GetComponent<RectTransform>().localPosition = new Vector3((gc.columns - 1) * spacing / 2, -currentRow * spacing, 0);
                selectors[i * 3 + 1].GetComponent<RectTransform>().localPosition = new Vector3(currentColumn * spacing, (1 - gc.rows) * spacing / 2, 0);
                if (currentRow >= 0) selectors[i * 3 + 0].SetActive(true);
                else selectors[i * 3 + 0].SetActive(false);
                if (currentColumn >= 0) selectors[i * 3 + 1].SetActive(true);
                else selectors[i * 3 + 1].SetActive(false);
                selectors[i * 3 + 2].SetActive(false);
            }
            else
            {
                selectors[i * 3 + 2].SetActive(true);
                selectors[i * 3 + 2].GetComponent<RectTransform>().localPosition = nodes[currentNode].GetComponent<RectTransform>().localPosition;
                selectors[i * 3 + 0].SetActive(false);
                selectors[i * 3 + 1].SetActive(false);
                foreach (GameObject node in nodes)
                {
                    node.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 0);
                }
            }
        }
    }
}
