using System;
using System.Linq;
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

    [HideInInspector]
    public UnityEvent cursorUpdate;
    [HideInInspector]
    public UnityEvent selectionUpdate;

    private int colors;

    private int[] selectedRow;
    private int[] selectedColumn;
    private int[] selectedNode;
    private int currentCursor;

    private bool[] skipped;
    private int[] singleNode;

    private bool confirmed;

    private float timer;

    void Start()
    {
        colors = InputProxy.playerCount;

        skipped = new bool[rows + columns];
        singleNode = new int[rows + columns];

        selectedRow = new int[colors];
        Array.Fill(selectedRow, -1);
        selectedColumn = new int[colors];
        Array.Fill(selectedColumn, -1);
        selectedNode = new int[colors];
        Array.Fill(selectedNode, -1);

        for (int i = 0; i < rows + columns; i++) {
            int disabled = 0;
            int nodeCount;
            if (i < rows)
            {
                nodeCount = Mathf.Min(columns, nodes.Length - i * columns);
            }
            else {
                nodeCount = Mathf.Min(rows, nodes.Length / (i + 1 - rows));
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
        if (skipped[currentCursor]) {
            UpdateCursor();
        }
        timer = delayTime;
    }

    void Update()
    {
        if (confirmed)
        {

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

        for (int i = 0; i < colors; i++) {
            if (InputProxy.instance[i]) {
                SelectCursor(i);
            }
        }
    }

    void SelectCursor(int color) {
        if (singleNode[currentCursor] >= 0) {
            selectedNode[color] = singleNode[currentCursor];
        }

        if (currentCursor < rows)
        {
            selectedRow[color] = currentCursor;
        }
        else
        {
            selectedColumn[color] = currentCursor - rows;
        }

        if (selectedColumn[color] >= 0 && selectedRow[color] >= 0) {
            selectedNode[color] = GetNode(selectedColumn[color], selectedRow[color]);
        }
        confirmed = selectedNode.All(i => i >= 0);
        selectionUpdate.Invoke();
    }

    void UpdateCursor()
    {
        currentCursor++;
        if (currentCursor >= rows + columns) {
            currentCursor = 0;
        }
        if (skipped[currentCursor]) {
            UpdateCursor();
        }
    }

    int GetNode(int row, int column) {
        return row * columns + column;
    }

    public int GetCursor() 
    {
        return currentCursor;
    }

    public int GetSelectedRow(int color)
    {
        return selectedRow[color];
    }

    public int GetSelectedColumn(int color) {
        return selectedColumn[color];
    }

    public int GetSelectedNode(int color) {
        return selectedNode[color];
    }
}
