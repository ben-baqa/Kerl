using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    public Sprite borderSprite;

    public int length;
    public float size;
    public float spacing;

    public bool vertical;

    private List<GameObject> pieces;

    public void DrawSelector(Color[] colors)
    {
        pieces = new List<GameObject>();
        for (int i = 0; i < colors.Length; i++)
        {
            GameObject piece = new GameObject("Selector Piece", typeof(Image));
            piece.GetComponent<RectTransform>().SetParent(transform);
            piece.GetComponent<RectTransform>().localPosition = Vector3.zero;

            piece.GetComponent<Image>().sprite = borderSprite;
            piece.GetComponent<Image>().color = colors[i];
            piece.GetComponent<Image>().type = Image.Type.Filled;
            if (vertical)
            {
                piece.GetComponent<RectTransform>().sizeDelta = new Vector3(size, size + spacing * (length - 1));
                piece.GetComponent<Image>().fillMethod = Image.FillMethod.Horizontal;
            }
            else
            {
                piece.GetComponent<RectTransform>().sizeDelta = new Vector3(size + spacing * (length - 1), size);
                piece.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
            }
            piece.GetComponent<Image>().fillAmount = (float)(colors.Length - i) / (float)(colors.Length);
            pieces.Add(piece);
        }
    }

    public void EraseSelector() {
        Destroy(gameObject);
    }
}
