using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorDrawer : MonoBehaviour
{
    public Sprite borderSprite;

    public float size;
    public float spacing;

    public int columns;
    public int rows;

    public List<Color> colors;
    public int[,] selections;

    private List<Selector> selectors;

    public void AddSelection(int color, bool isRow, int index) {
        if (selections == null) {
            selections = new int[InputProxy.playerCount, 2];
            for (int i = 0; i < InputProxy.playerCount; i++)
            {
                selections[i, 0] = -1;
                selections[i, 1] = -1;
            }
        }
        if (isRow)
        {
            selections[color, 0] = index;
        }
        else
        {
            selections[color, 1] = index;
        }
    }

    public void DrawSelectors() {
        if (selectors == null) {
            selectors = new List<Selector>();
        }
        for (int i = 0; i < selectors.Count; i++)
        {
            selectors[i].EraseSelector();
        }
        selectors = new List<Selector>();
        for (int i = 0; i < rows + columns; i++) {
            if (i < rows)
            {
                GameObject selectorObject = new GameObject("Selector", typeof(Selector));
                Selector selector = selectorObject.GetComponent<Selector>();
                selector.borderSprite = borderSprite;
                selector.length = columns;
                selector.size = size;
                selector.spacing = spacing;
                selector.vertical = false;
                selectorObject.transform.SetParent(transform);
                selectorObject.transform.localPosition = new Vector2(spacing * (columns - 1) / 2, -i * spacing);
                List<Color> selectorColors = new List<Color>();
                for (int j = 0; j < InputProxy.playerCount; j++)
                {
                    if (selections[j, 0] == i)
                    {
                        selectorColors.Add(colors[j]);
                    }
                }
                selector.DrawSelector(selectorColors.ToArray());
                selectors.Add(selector);
            }
            else {
                GameObject selectorObject = new GameObject("Selector", typeof(Selector));
                Selector selector = selectorObject.GetComponent<Selector>();
                selector.borderSprite = borderSprite;
                selector.length = rows;
                selector.size = size;
                selector.spacing = spacing;
                selector.vertical = true;
                selectorObject.transform.SetParent(transform);
                selectorObject.transform.localPosition = new Vector2((i - rows) * spacing, -spacing * (rows - 1) / 2);
                List<Color> selectorColors = new List<Color>();
                for (int j = 0; j < InputProxy.playerCount; j++)
                {
                    if (selections[j, 1] == i - rows)
                    {
                        selectorColors.Add(colors[j]);
                    }
                }
                selector.DrawSelector(selectorColors.ToArray());
                selectors.Add(selector);
            }
        }
    }
}
