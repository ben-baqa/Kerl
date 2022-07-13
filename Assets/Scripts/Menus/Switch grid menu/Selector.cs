using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    List<GameObject> pieces;
    Sprite borderSprite;

    float size;
    float spacing;
    int length;
    int height;
    bool isVertical;

    public void Init(Sprite border, NodePlacementSettings placementSettings, bool vertical, Transform parent)
    {
        borderSprite = border;
        length = placementSettings.columns;
        height = placementSettings.rows;
        size = placementSettings.selectorSize;
        spacing = placementSettings.spacing;

        isVertical = vertical;

        transform.SetParent(parent, false);
    }

    public void DrawSelector(Color[] colors, Vector2 position)
    {
        transform.localPosition = position;

        pieces = new List<GameObject>();
        for (int i = 0; i < colors.Length; i++)
        {
            GameObject piece = new GameObject("Selector Piece");
            piece.transform.SetParent(transform);
            piece.transform.localPosition = Vector3.zero;
            piece.transform.localScale = Vector3.one;

            Image im = piece.AddComponent<Image>();
            im.sprite = borderSprite;
            im.color = colors[i];
            im.type = Image.Type.Filled;
            if (isVertical)
            {
                piece.GetComponent<RectTransform>().sizeDelta = new Vector3(size, size + spacing * (height - 1));
                im.fillMethod = Image.FillMethod.Horizontal;
            }
            else
            {
                piece.GetComponent<RectTransform>().sizeDelta = new Vector3(size + spacing * (length - 1), size);
                im.fillMethod = Image.FillMethod.Vertical;
            }
            im.fillAmount = (float)(colors.Length - i) / (float)(colors.Length);
            pieces.Add(piece);
        }
    }

    public void EraseSelector() {
        Destroy(gameObject);
    }
}
