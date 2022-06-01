using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GlobalCursor : MonoBehaviour
{
    private enum State
    {
        Select,
        Confirm,
        Done
    }

    private enum SelectType
    {
        Columns,
        Rows,
        Both
    }

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

    private bool[] confirmed;
    private bool[] skipped;
    private int[] singleNode;

    private SelectType selectType;
    private State state;

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
        confirmed = new bool[colors];
        Array.Fill(confirmed, false);

        for (int i = 0; i < rows + columns; i++)
        {
            int disabled = 0;
            int nodeCount;
            if (i < rows)
            {
                nodeCount = Mathf.Min(columns, nodes.Length - i * columns);
            }
            else
            {
                nodeCount = Mathf.Min(rows, nodes.Length / (i + 1 - rows));
            }
            for (int j = 0; j < nodeCount; j++)
            {
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
                else
                {
                    singleNode[i] = currentNode;
                }
            }
            if (disabled == nodeCount)
            {
                skipped[i] = true;
            }
            if (nodeCount - disabled != 1)
            {
                singleNode[i] = -1;
            }
        }

        selectType = SelectType.Both;
        if (rows == 1 || Enumerable.Range(rows - 1, columns).All(i => skipped[i] || singleNode[i] >= 0)) selectType = SelectType.Columns;
        else if (columns == 1 || Enumerable.Range(0, rows).All(i => skipped[i] || singleNode[i] >= 0)) selectType = SelectType.Rows;

        if (skipped[currentCursor] || selectType == SelectType.Columns)
        {
            UpdateCursor();
        }
        timer = delayTime;
    }

    void Update()
    {
        if (state != State.Done)
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

        for (int i = 0; i < colors; i++)
        {
            if (InputProxy.instance[i])
            {
                if (state == State.Select) SelectCursor(i);
                else if (state == State.Confirm) ConfirmCursor(i);
            }
        }
    }

    void SelectCursor(int color)
    {
        if (singleNode[currentCursor] >= 0)
        {
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

        if (selectedColumn[color] >= 0 && selectedRow[color] >= 0)
        {
            selectedNode[color] = GetNode(selectedRow[color], selectedColumn[color]);
            if (nodes[selectedNode[color]]) {
                ResetSelection(color);
            }
        }

        if (selectedNode.All(i => i >= 0))
        {
            state = State.Confirm;
            currentCursor = 0;
        }
        selectionUpdate.Invoke();
    }

    void ConfirmCursor(int color)
    {
        if (currentCursor == 0)
        {
            state = State.Select;
            ResetSelection(color);
            Array.Fill(confirmed, false);
            return;
        }
        confirmed[color] = true;
        if (confirmed.All(c =>  c)) {
            state = State.Done;
        }
    }

    void UpdateCursor()
    {
        currentCursor++;
        if (state == State.Select)
        {
            if (selectType == SelectType.Columns && currentCursor < rows - 1)
            {
                currentCursor = rows - 1;
            }
            else if (selectType == SelectType.Rows && currentCursor >= rows)
            {
                currentCursor = 0;
            }
            else if (currentCursor >= rows + columns)
            {
                currentCursor = selectType == SelectType.Columns ? rows - 1 : 0;
            }
            if (skipped[currentCursor])
            {
                UpdateCursor();
            }

        }
        else if (state == State.Confirm)
        {
            if (currentCursor > 1)
            {
                currentCursor = 0;
            }
        }

    }

    int GetNode(int row, int column)
    {
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

    public int GetSelectedColumn(int color)
    {
        return selectedColumn[color];
    }

    public int GetSelectedNode(int color)
    {
        return selectedNode[color];
    }

    public void ResetSelection(int color)
    {
        selectedColumn[color] = -1;
        selectedRow[color] = -1;
        selectedNode[color] = -1;
    }
}
