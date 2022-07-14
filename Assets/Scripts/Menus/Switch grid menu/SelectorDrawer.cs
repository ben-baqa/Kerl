using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorDrawer : MonoBehaviour
{
    List<Selector> selectors;
    Sprite borderSprite;

    int[,] selections;

    Vector2 offset;
    float spacing;

    int columns;
    int rows;

    NodePlacementSettings placementSettings;

    public void Init(Sprite border, NodePlacementSettings placementSettings, Transform parent)
    {
        this.placementSettings = placementSettings;

        borderSprite = border;
        columns = placementSettings.columns;
        rows = placementSettings.rows;
        spacing = placementSettings.spacing;
        offset = placementSettings.Offset;

        transform.SetParent(parent, false);
    }

    public void AddSelection(int playerIndex, bool isRow, int index) {
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
            selections[playerIndex, 0] = index;
        }
        else
        {
            selections[playerIndex, 1] = index;
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
                GameObject selectorObject = new GameObject("Selector");
                Selector selector = selectorObject.AddComponent<Selector>();
                selector.Init(borderSprite, placementSettings, false, transform);
                    
                Vector2 selectorPosition = new Vector2(spacing * (columns - 1) / 2f, -i * spacing);

                List<Color> selectorColors = new List<Color>();
                for (int j = 0; j < InputProxy.playerCount; j++)
                    if (selections[j, 0] == i)
                        selectorColors.Add(MenuSelections.GetColor(j));

                selector.DrawSelector(selectorColors.ToArray(), selectorPosition + offset);
                selectors.Add(selector);
            }
            else {
                GameObject selectorObject = new GameObject("Selector");
                Selector selector = selectorObject.AddComponent<Selector>();
                selector.Init(borderSprite, placementSettings, true, transform);
                    
                Vector2 selectorPosition = new Vector2((i - rows) * spacing, -spacing * (rows - 1) / 2f);

                List<Color> selectorColors = new List<Color>();
                for (int j = 0; j < InputProxy.playerCount; j++)
                    if (selections[j, 1] == i - rows)
                        selectorColors.Add(MenuSelections.GetColor(j));

                selector.DrawSelector(selectorColors.ToArray(), selectorPosition + offset);
                selectors.Add(selector);
            }
        }
    }
}
