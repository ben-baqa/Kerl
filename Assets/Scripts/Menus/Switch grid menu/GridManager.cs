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

    private GlobalCursor gc;

    private List<GameObject> nodes;

    void Start()
    {
        gc = gameObject.GetComponent<GlobalCursor>();
        nodes = new List<GameObject>();
        for (int i = 0; i < gc.rows; i++) {
            for (int j = 0; j < gc.columns; j++)
            {
                int currentNode = i * gc.columns + j;
                if (currentNode >= gc.nodes.Length) {
                    break;
                }
                GameObject newNode = new GameObject("node", typeof(Image));
                newNode.GetComponent<RectTransform>().SetParent(transform);
                newNode.GetComponent<RectTransform>().sizeDelta = new Vector3(nodeSize, nodeSize, 0);
                newNode.GetComponent<RectTransform>().localPosition = new Vector3(spacing * j, -spacing * i, 0);
                newNode.GetComponent<Image>().sprite = sprite;
                if (gc.nodes[currentNode])
                {
                    newNode.GetComponent<Image>().color = Color.gray;
                }
                else {
                    newNode.GetComponent<Image>().color = Color.white;
                }
                nodes.Add(newNode);
            }
        }
    }

    void Update()
    {
        
    }
}
