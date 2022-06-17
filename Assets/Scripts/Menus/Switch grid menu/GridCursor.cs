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

    public int Columns;
    public int Rows;
    public bool[] Nodes;

    public float DelayTime;

    [HideInInspector]
    public UnityEvent CursorUpdate = new UnityEvent();
    [HideInInspector]
    public UnityEvent SelectionUpdate = new UnityEvent();

    private int _colors;

    private int[] _selectedRow;
    private int[] _selectedColumn;
    private int[] _selectedNode;
    private int _currentCursor;

    private bool[] _confirmed;
    private bool[] _skipped;
    private int[] _singleNode;

    private SelectType _selectType;
    private State _state;

    private float _timer;

    void Start()
    {
        _colors = InputProxy.playerCount;

        _skipped = new bool[Rows + Columns];
        _singleNode = new int[Rows + Columns];

        _selectedRow = new int[_colors];
        Array.Fill(_selectedRow, -1);
        _selectedColumn = new int[_colors];
        Array.Fill(_selectedColumn, -1);
        _selectedNode = new int[_colors];
        Array.Fill(_selectedNode, -1);
        _confirmed = new bool[_colors];
        Array.Fill(_confirmed, false);

        for (int i = 0; i < Rows + Columns; i++)
        {
            int disabled = 0;
            int nodeCount;
            if (i < Rows)
            {
                nodeCount = Mathf.Min(Columns, Nodes.Length - i * Columns);
            }
            else
            {
                nodeCount = Mathf.Min(Rows, Nodes.Length / (i + 1 - Rows));
            }
            for (int j = 0; j < nodeCount; j++)
            {
                int currentNode;
                if (i < Rows)
                {
                    currentNode = i * Columns + j;
                }
                else
                {
                    currentNode = j * Columns + i - Rows;
                }
                if (Nodes[currentNode])
                {
                    disabled++;
                }
                else
                {
                    _singleNode[i] = currentNode;
                }
            }
            if (disabled == nodeCount)
            {
                _skipped[i] = true;
            }
            if (nodeCount - disabled != 1)
            {
                _singleNode[i] = -1;
            }
        }

        _selectType = SelectType.Both;
        if (Rows == 1 || Enumerable.Range(Rows - 1, Columns).All(i => _skipped[i] || _singleNode[i] >= 0)) _selectType = SelectType.Columns;
        else if (Columns == 1 || Enumerable.Range(0, Rows).All(i => _skipped[i] || _singleNode[i] >= 0)) _selectType = SelectType.Rows;

        if (_skipped[_currentCursor] || _selectType == SelectType.Columns)
        {
            UpdateCursor();
        }
        _timer = DelayTime;
    }

    void Update()
    {
        if (_state != State.Done)
        {
            if (_timer <= 0)
            {
                _timer = DelayTime;
                UpdateCursor();
                CursorUpdate.Invoke();
            }
            else
            {
                _timer -= Time.deltaTime;
            }
        }

        for (int i = 0; i < _colors; i++)
        {
            if (InputProxy.GetToggledInput(i))
            {
                if (_state == State.Select) SelectCursor(i);
                else if (_state == State.Confirm) ConfirmCursor(i);
            }
        }
    }

    void SelectCursor(int color)
    {
        if (_singleNode[_currentCursor] >= 0)
        {
            _selectedNode[color] = _singleNode[_currentCursor];
        }

        if (_currentCursor < Rows)
        {
            _selectedRow[color] = _currentCursor;
        }
        else
        {
            _selectedColumn[color] = _currentCursor - Rows;
        }

        if (_selectedColumn[color] >= 0 && _selectedRow[color] >= 0)
        {
            _selectedNode[color] = GetNode(_selectedRow[color], _selectedColumn[color]);
            if (Nodes.Length > _selectedNode[color]) if (Nodes[_selectedNode[color]]) ResetSelection(color);
        }

        if (_selectedNode.All(i => i >= 0))
        {
            _state = State.Confirm;
            _currentCursor = 0;
        }
        SelectionUpdate.Invoke();
    }

    void ConfirmCursor(int color)
    {
        if (_currentCursor == 0)
        {
            _state = State.Select;
            ResetSelection(color);
            Array.Fill(_confirmed, false);
            return;
        }
        _confirmed[color] = true;
        if (_confirmed.All(c => c))
        {
            _state = State.Done;
        }
    }

    void UpdateCursor()
    {
        _currentCursor++;
        if (_state == State.Select)
        {
            if (_selectType == SelectType.Columns && _currentCursor < Rows - 1)
            {
                _currentCursor = Rows - 1;
            }
            else if (_selectType == SelectType.Rows && _currentCursor >= Rows)
            {
                _currentCursor = 0;
            }
            else if (_currentCursor >= Rows + Columns)
            {
                _currentCursor = _selectType == SelectType.Columns ? Rows - 1 : 0;
            }
            if (_skipped[_currentCursor])
            {
                UpdateCursor();
            }

        }
        else if (_state == State.Confirm)
        {
            if (_currentCursor > 1)
            {
                _currentCursor = 0;
            }
        }

    }

    int GetNode(int row, int column)
    {
        return row * Columns + column;
    }

    public int GetCursor()
    {
        return _currentCursor;
    }

    public int GetSelectedRow(int color)
    {
        return _selectedRow[color];
    }

    public int GetSelectedColumn(int color)
    {
        return _selectedColumn[color];
    }

    public int GetSelectedNode(int color)
    {
        return _selectedNode[color];
    }

    public void ResetSelection(int color)
    {
        _selectedColumn[color] = -1;
        _selectedRow[color] = -1;
        _selectedNode[color] = -1;
    }
}
