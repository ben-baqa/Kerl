using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorDrawer : MonoBehaviour
{
    public Sprite BorderSprite;

    public float Size;
    public float Spacing;

    public int Columns;
    public int Rows;

    public List<Color> Colors;
    public int[,] Selections;

    private List<Selector> _selectors;

    public void AddSelection(int color, bool isRow, int index) {
        if (Selections == null) {
            Selections = new int[InputProxy.playerCount, 2];
            for (int i = 0; i < InputProxy.playerCount; i++)
            {
                Selections[i, 0] = -1;
                Selections[i, 1] = -1;
            }
        }
        if (isRow)
        {
            Selections[color, 0] = index;
        }
        else
        {
            Selections[color, 1] = index;
        }
    }

    public void DrawSelectors() {
        if (_selectors == null) {
            _selectors = new List<Selector>();
        }
        for (int i = 0; i < _selectors.Count; i++)
        {
            _selectors[i].EraseSelector();
        }
        _selectors = new List<Selector>();
        for (int i = 0; i < Rows + Columns; i++) {
            if (i < Rows)
            {
                GameObject selectorObject = new GameObject("Selector", typeof(Selector));
                Selector selector = selectorObject.GetComponent<Selector>();
                selector.BorderSprite = BorderSprite;
                selector.Length = Columns;
                selector.Size = Size;
                selector.Spacing = Spacing;
                selector.IsVertical = false;
                selectorObject.transform.SetParent(transform);
                selectorObject.transform.localPosition = new Vector2(Spacing * (Columns - 1) / 2, -i * Spacing);
                List<Color> selectorColors = new List<Color>();
                for (int j = 0; j < InputProxy.playerCount; j++)
                {
                    if (Selections[j, 0] == i)
                    {
                        selectorColors.Add(Colors[j]);
                    }
                }
                selector.DrawSelector(selectorColors.ToArray());
                _selectors.Add(selector);
            }
            else {
                GameObject selectorObject = new GameObject("Selector", typeof(Selector));
                Selector selector = selectorObject.GetComponent<Selector>();
                selector.BorderSprite = BorderSprite;
                selector.Length = Rows;
                selector.Size = Size;
                selector.Spacing = Spacing;
                selector.IsVertical = true;
                selectorObject.transform.SetParent(transform);
                selectorObject.transform.localPosition = new Vector2((i - Rows) * Spacing, -Spacing * (Rows - 1) / 2);
                List<Color> selectorColors = new List<Color>();
                for (int j = 0; j < InputProxy.playerCount; j++)
                {
                    if (Selections[j, 1] == i - Rows)
                    {
                        selectorColors.Add(Colors[j]);
                    }
                }
                selector.DrawSelector(selectorColors.ToArray());
                _selectors.Add(selector);
            }
        }
    }
}
