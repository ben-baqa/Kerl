using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    public Sprite BorderSprite;

    public int Length;
    public float Size;
    public float Spacing;

    public bool IsVertical;

    private List<GameObject> _pieces;

    public void DrawSelector(Color[] colors)
    {
        _pieces = new List<GameObject>();
        for (int i = 0; i < colors.Length; i++)
        {
            GameObject piece = new GameObject("Selector Piece", typeof(Image));
            piece.GetComponent<RectTransform>().SetParent(transform);
            piece.GetComponent<RectTransform>().localPosition = Vector3.zero;

            piece.GetComponent<Image>().sprite = BorderSprite;
            piece.GetComponent<Image>().color = colors[i];
            piece.GetComponent<Image>().type = Image.Type.Filled;
            if (IsVertical)
            {
                piece.GetComponent<RectTransform>().sizeDelta = new Vector3(Size, Size + Spacing * (Length - 1));
                piece.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
            }
            else
            {
                piece.GetComponent<RectTransform>().sizeDelta = new Vector3(Size + Spacing * (Length - 1), Size);
                piece.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
            }
            piece.GetComponent<Image>().fillAmount = (float)(colors.Length - i) / (float)(colors.Length);
            _pieces.Add(piece);
        }
    }

    public void EraseSelector() {
        Destroy(gameObject);
    }
}
