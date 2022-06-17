using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public Sprite BorderSprite;
    public Sprite ImageSprite;

    public float Size;
    public float ImageSize;
    public float SelectedRatio;

    public bool Disabled;

    private GameObject _image;
    private GameObject _border;
    private List<GameObject> _frame;

    private bool selected;

    void Start()
    {
        CreateNode(new Color[] { });
    }

    void Update()
    {
        if (selected)
        {
            foreach (GameObject piece in _frame)
            {
                piece.GetComponent<RectTransform>().localScale = SelectedRatio * Vector2.one;
            }
            _image.GetComponent<RectTransform>().localScale = SelectedRatio * Vector2.one;
            _border.GetComponent<RectTransform>().localScale = SelectedRatio * Vector2.one;
        }
        else {
            foreach (GameObject piece in _frame)
            {
                piece.GetComponent<RectTransform>().localScale = Vector2.one;
            }
            _image.GetComponent<RectTransform>().localScale = Vector2.one;
            _border.GetComponent<RectTransform>().localScale = Vector2.one;
        }
    }

    public void ChangeColor(Color[] colors) {
        DestroyCurrent();
        CreateNode(colors);
    }

    void CreateNode(Color[] colors) {
        _frame = new List<GameObject>();
        for (int i = 0; i < colors.Length; i++) {
            GameObject framePiece = new GameObject("Frame Piece", typeof(Image));
            framePiece.GetComponent<RectTransform>().SetParent(transform);
            framePiece.GetComponent<RectTransform>().localPosition = Vector3.zero;
            framePiece.GetComponent<RectTransform>().sizeDelta = Size * Vector2.one;
            framePiece.GetComponent<Image>().sprite = BorderSprite;
            framePiece.GetComponent<Image>().color = colors[i];
            framePiece.GetComponent<Image>().type = Image.Type.Filled;
            framePiece.GetComponent<Image>().fillAmount = (float)(colors.Length - i) / (float)(colors.Length);
            _frame.Add(framePiece);
        }
        _border = new GameObject("Border", typeof(Image));
        _border.GetComponent<RectTransform>().SetParent(transform);
        _border.GetComponent<RectTransform>().localPosition = Vector3.zero;
        if (colors.Length > 0) _border.GetComponent<RectTransform>().sizeDelta = ImageSize * Vector2.one;
        else _border.GetComponent<RectTransform>().sizeDelta = Size * Vector2.one;
        _border.GetComponent<Image>().sprite = BorderSprite;
        _border.GetComponent<Image>().color = Disabled ? Color.gray : Color.white;

        _image = new GameObject("Image", typeof(Image));
        _image.GetComponent<RectTransform>().SetParent(transform);
        _image.GetComponent<RectTransform>().localPosition = Vector3.zero;
        if (colors.Length > 0) _image.GetComponent<RectTransform>().sizeDelta = ImageSize * Vector2.one;
        else _image.GetComponent<RectTransform>().sizeDelta = Size * Vector2.one;
        _image.GetComponent<Image>().sprite = ImageSprite;
        _image.GetComponent<Image>().color = Disabled ? new Color(1, 1, 1, 0.3f) : Color.white;
    }

    void DestroyCurrent() {
        if (_image != null)
        {
            Destroy(_image);
        }
        if (_border != null)
        {
            Destroy(_border);
        }
        foreach (GameObject piece in _frame)
        {
            Destroy(piece);
        }
    }

    public void SetSelected(bool isSelected) {
        selected = isSelected;
    }
}
