using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GridCursor : MonoBehaviour
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

    Action updateVisualCursor;
    Action updateVisualSelection;
    Action onConfirmationStarted;
    Action onReject;
    Action<int[]> onConfirm;

    GridConfirmationCursor confirmationCursor;

    int columns;
    int rows;
    bool[] locked;

    int playerCount;

    int[] selectedRow;
    int[] selectedColumn;
    int[] selectedNode;
    int currentCursor;

    bool[] confirmed;
    bool[] skipped;
    int[] singleNode;

    SelectType selectType;
    State state;

    float period;
    float timer;

    public void Init(int columns, int rows, bool[] locked, float period,
        Action updateCursor, Action updateSelection,
        Action enterConfirmation, Action reject, Action<int[]> confirm)
    {
        this.columns = columns;
        this.rows = rows;
        this.locked = locked;
        this.period = period;
        updateVisualCursor = updateCursor;
        updateVisualSelection = updateSelection;
        onConfirmationStarted = enterConfirmation;
        onReject = reject;
        onConfirm = confirm;

        playerCount = InputProxy.playerCount;

        skipped = new bool[rows + columns];
        singleNode = new int[rows + columns];

        selectedRow = new int[playerCount];
        Array.Fill(selectedRow, -1);
        selectedColumn = new int[playerCount];
        Array.Fill(selectedColumn, -1);
        selectedNode = new int[playerCount];
        Array.Fill(selectedNode, -1);
        confirmed = new bool[playerCount];
        Array.Fill(confirmed, false);

        FindSkippableNodes();

        selectType = SelectType.Both;
        if (rows == 1 || Enumerable.Range(rows - 1, columns).All(i => skipped[i] || singleNode[i] >= 0))
            selectType = SelectType.Columns;
        else if (columns == 1 || Enumerable.Range(0, rows).All(i => skipped[i] || singleNode[i] >= 0))
            selectType = SelectType.Rows;

        if (skipped[currentCursor] || selectType == SelectType.Columns)
        {
            UpdateCursor();
        }
        timer = period;
    }

    void FindSkippableNodes()
    {

        for (int i = 0; i < rows + columns; i++)
        {
            int disabled = 0;
            int nodeCount;
            if (i < rows)
                nodeCount = Mathf.Min(columns, locked.Length - i * columns);
            else
                nodeCount = Mathf.Min(rows, locked.Length / (i + 1 - rows));

            for (int j = 0; j < nodeCount; j++)
            {
                int currentNode;
                if (i < rows)
                    currentNode = i * columns + j;
                else
                    currentNode = j * columns + i - rows;

                if (locked[currentNode])
                    disabled++;
                else
                    singleNode[i] = currentNode;
            }

            if (disabled == nodeCount)
                skipped[i] = true;

            if (nodeCount - disabled != 1)
                singleNode[i] = -1;
        }
    }

    void Update()
    {
        if (state != State.Done)
        {
            if (timer <= 0)
            {
                timer = period;
                UpdateCursor();
                if (state == State.Select)
                    updateVisualCursor();
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        for (int i = 0; i < playerCount; i++)
        {
            if (InputProxy.GetToggledInput(i))
            {
                if (state == State.Select)
                    SelectCursor(i);
                else if (state == State.Confirm)
                    ConfirmCursor(i);
            }
        }
    }

    void SelectCursor(int playerIndex)
    {
        if (singleNode[currentCursor] >= 0)
            selectedNode[playerIndex] = singleNode[currentCursor];

        if (currentCursor < rows)
            selectedRow[playerIndex] = currentCursor;
        else
            selectedColumn[playerIndex] = currentCursor - rows;

        if (selectedColumn[playerIndex] >= 0 && selectedRow[playerIndex] >= 0)
        {
            selectedNode[playerIndex] = GetNode(selectedRow[playerIndex], selectedColumn[playerIndex]);
            //if (locked.Length > selectedNode[color]) if (locked[selectedNode[color]]) ResetSelection(color);
            if (locked.Length > selectedNode[playerIndex] && locked[selectedNode[playerIndex]])
                ResetSelection(playerIndex);
        }

        if (selectedNode.All(i => i >= 0))
        {
            onConfirmationStarted();
            state = State.Confirm;
            currentCursor = 0;
            if (confirmationCursor == null)
                confirmationCursor = GetComponentInChildren<GridConfirmationCursor>();
            confirmationCursor.UpdateCursor(currentCursor != 0);
        }
        updateVisualSelection();
    }

    void ConfirmCursor(int playerIndex)
    {
        if (currentCursor == 0)
        {
            state = State.Select;
            Array.Fill(confirmed, false);
            ResetSelection(playerIndex);
            confirmationCursor.Hide();
            updateVisualSelection();
            onReject();
            return;
        }
        confirmed[playerIndex] = true;
        if (confirmed.All(c => c))
        {
            state = State.Done;
            onConfirm(selectedNode);
        }
    }

    void UpdateCursor()
    {
        currentCursor++;
        if (state == State.Select)
        {
            if (selectType == SelectType.Columns && currentCursor < rows - 1)
                currentCursor = rows - 1;
            else if (selectType == SelectType.Rows && currentCursor >= rows)
                currentCursor = 0;
            else if (currentCursor >= rows + columns)
                currentCursor = selectType == SelectType.Columns ? rows - 1 : 0;

            if (skipped[currentCursor])
                UpdateCursor();

        }
        else if (state == State.Confirm)
        {
            if (currentCursor > 1)
                currentCursor = 0;

            confirmationCursor.UpdateCursor(currentCursor != 0);
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
