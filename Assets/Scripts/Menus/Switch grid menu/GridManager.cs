using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GameObject selectorPrefab;
    public GameObject nodePrefab;

    public int rows = 3;
    public int columns = 3;

    public float spacing = 100;

    public float delayTime = 1;

    private GameObject[] selector = new GameObject[2];
    private List<GameObject> node = new List<GameObject>();

    private int currentRow;
    private int currentColumn;

    private bool rowConfirmed;
    private bool columnConfirmed;

    private float timer;

    void Start()
    {
        GetComponent<RectTransform>().localPosition = new Vector3(-spacing * (columns - 1) / 2, -spacing * (rows - 1) / 2, 0);

        timer = delayTime;

        for (int i = 0; i < 2; i++) {
            selector[i] = Instantiate(selectorPrefab);
            selector[i].GetComponent<RectTransform>().parent = GetComponent<RectTransform>();
        }

        selector[0].GetComponent<RectTransform>().sizeDelta = new Vector3(120 + spacing * (columns - 1), 120, 0);
        selector[1].GetComponent<RectTransform>().sizeDelta = new Vector3(120, 120 + spacing * (rows - 1), 0);
        UpdateSelector();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject newNode = Instantiate(nodePrefab);
                newNode.GetComponent<RectTransform>().parent = GetComponent<RectTransform>();
                newNode.GetComponent<RectTransform>().localPosition = new Vector3(spacing * j, spacing * i, 0);
                node.Add(newNode);
            }
        }
    }

    void Update()
    {
        if (rowConfirmed && columnConfirmed)
        {
            for (int i = 0; i < 2; i++)
            {
                selector[i].SetActive(false);
            }
            node[currentRow * columns + currentColumn].GetComponent<Image>().color = Color.red;
        }
        else
        {
            if (timer <= 0)
            {
                timer = delayTime;
                if (rowConfirmed)
                {
                    currentColumn++;
                    if (currentColumn >= columns)
                    {
                        currentColumn = 0;
                    }
                }
                else {
                    currentRow++;
                    if (currentRow >= rows)
                    {
                        currentRow = 0;
                    }
                }

                UpdateSelector();
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        if (Input.GetButtonDown("Submit")) {
            if (rowConfirmed)
            {
                columnConfirmed = true;
            }
            else {
                rowConfirmed = true;
            }
        }
    }

    void UpdateSelector()
    {
        selector[0].GetComponent<RectTransform>().localPosition = new Vector3(spacing * (columns - 1) / 2, spacing * currentRow, 0);
        selector[1].GetComponent<RectTransform>().localPosition = new Vector3(spacing * currentColumn, spacing * (rows - 1) / 2, 0);
        selector[1].SetActive(rowConfirmed);
    }
}
