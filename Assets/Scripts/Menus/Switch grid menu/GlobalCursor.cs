using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalCursor : MonoBehaviour
{
    public int rows;
    public int columns;
    public bool[] nodes;

    public float delayTime;

    public UnityEvent cursorUpdate;
    public UnityEvent selectionUpdate;

    private int selectedRow;
    private int selectedColumn;
    private int selectedNode;
    private int currentCursor;

    private bool[] skipped;
    private int[] singleNode;

    private bool rowConfirmed;
    private bool columnConfirmed;

    private float timer;

    void Start()
    {
        skipped = new bool[rows + columns];
        singleNode = new int[rows + columns];
        for (int i = 0; i < rows + columns; i++) {
            int disabled = 0;
            int nodeCount;
            if (i < rows)
            {
                nodeCount = Mathf.Min(columns, nodes.Length - i * columns - 1);
            }
            else {
                nodeCount = Mathf.Min(rows, (nodes.Length) / (i + 1 - rows));
            }
            for (int j = 0; j < nodeCount; j++) {
                int currentNode;
                if (i < rows)
                {
                    currentNode = i * columns + j;
                }
                else
                {
                    currentNode = j * columns + i - rows;
                }
                if (nodes[currentNode])
                {
                    disabled++;
                }
                else {
                    singleNode[i] = currentNode;
                }
            }
            if (disabled == nodeCount)
            {
                skipped[i] = true;
            }
            if (nodeCount - disabled != 1) {
                singleNode[i] = -1;
            }
        }
        timer = delayTime;
    }

    void Update()
    {
        if (rowConfirmed && columnConfirmed)
        {
            if (singleNode[currentCursor] > 0)
            {
                selectedNode = singleNode[currentCursor];
            }
            else {
                selectedNode = GetNode(selectedRow, selectedColumn);
                selectionUpdate.Invoke();
            }
        }
        else
        {
            if (timer <= 0)
            {
                timer = delayTime;
                UpdateCursor();
                cursorUpdate.Invoke();
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        if (Input.GetButton("Submit")) {
            SelectCursor();
        }
    }

    void SelectCursor() {
        if (singleNode[currentCursor] > 0) {
            selectedNode = singleNode[currentCursor];
            rowConfirmed = true;
            columnConfirmed = true;
        }

        if (currentCursor < rows)
        {
            selectedRow = currentCursor;
            rowConfirmed = true;
        }
        else
        {
            selectedColumn = currentCursor - rows;
            columnConfirmed = true;
        }
        selectionUpdate.Invoke();
    }

    void UpdateCursor()
    {
        currentCursor++;
        if (currentCursor >= rows + columns) {
            currentCursor = 0;
        }
        if (skipped[currentCursor]) {
            currentCursor++;
        }
    }

    int GetNode(int row, int column) {
        return row * columns + column;
    }

    public int GetCursor() 
    {
        return currentCursor;
    }

    public int GetSelectedRow()
    {
        if (!rowConfirmed) return -1;
        return selectedRow;
    }

    public int GetSelectedColumn() {
        if (!columnConfirmed) return -1;
        return selectedColumn;
    }

    public int GetSelectedNode() {
        if (rowConfirmed && columnConfirmed) return selectedNode;
        return -1;
    }
}
