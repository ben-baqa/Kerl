using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public Sprite borderSprite;
    public Sprite imageSprite;

    public float size;
    public float imageSize;
    public float selectedRatio;

    public bool disabled;

    private GameObject image;
    private GameObject border;
    private List<GameObject> frame;

    private bool selected;

    void Start()
    {
        CreateNode(new Color[] { });
    }

    void Update()
    {
        if (selected)
        {
            foreach (GameObject piece in frame)
            {
                piece.GetComponent<RectTransform>().localScale = selectedRatio * Vector2.one;
            }
            image.GetComponent<RectTransform>().localScale = selectedRatio * Vector2.one;
            border.GetComponent<RectTransform>().localScale = selectedRatio * Vector2.one;
        }
        else {
            foreach (GameObject piece in frame)
            {
                piece.GetComponent<RectTransform>().localScale = Vector2.one;
            }
            image.GetComponent<RectTransform>().localScale = Vector2.one;
            border.GetComponent<RectTransform>().localScale = Vector2.one;
        }
    }

    public void ChangeColor(Color[] colors) {
        DestroyCurrent();
        CreateNode(colors);
    }

    void CreateNode(Color[] colors) {
        frame = new List<GameObject>();
        for (int i = 0; i < colors.Length; i++) {
            GameObject framePiece = new GameObject("Frame Piece", typeof(Image));
            framePiece.GetComponent<RectTransform>().SetParent(transform);
            framePiece.GetComponent<RectTransform>().localPosition = Vector3.zero;
            framePiece.GetComponent<RectTransform>().sizeDelta = size * Vector2.one;
            framePiece.GetComponent<Image>().sprite = borderSprite;
            framePiece.GetComponent<Image>().color = colors[i];
            framePiece.GetComponent<Image>().type = Image.Type.Filled;
            framePiece.GetComponent<Image>().fillAmount = (float)(colors.Length - i) / (float)(colors.Length);
            frame.Add(framePiece);
        }
        border = new GameObject("Border", typeof(Image));
        border.GetComponent<RectTransform>().SetParent(transform);
        border.GetComponent<RectTransform>().localPosition = Vector3.zero;
        if (colors.Length > 0) border.GetComponent<RectTransform>().sizeDelta = imageSize * Vector2.one;
        else border.GetComponent<RectTransform>().sizeDelta = size * Vector2.one;
        border.GetComponent<Image>().sprite = borderSprite;
        border.GetComponent<Image>().color = disabled ? Color.gray : Color.white;

        image = new GameObject("Image", typeof(Image));
        image.GetComponent<RectTransform>().SetParent(transform);
        image.GetComponent<RectTransform>().localPosition = Vector3.zero;
        if (colors.Length > 0) image.GetComponent<RectTransform>().sizeDelta = imageSize * Vector2.one;
        else image.GetComponent<RectTransform>().sizeDelta = size * Vector2.one;
        image.GetComponent<Image>().sprite = imageSprite;
        image.GetComponent<Image>().color = disabled ? new Color(1, 1, 1, 0.3f) : Color.white;
    }

    void DestroyCurrent() {
        if (image != null)
        {
            Destroy(image);
        }
        if (border != null)
        {
            Destroy(border);
        }
        foreach (GameObject piece in frame)
        {
            Destroy(piece);
        }
    }

    public void SetSelected(bool isSelected) {
        selected = isSelected;
    }
}
