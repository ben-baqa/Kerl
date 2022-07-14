using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    Image image;
    Image border;
    List<GameObject> framePieces;

    Sprite borderSprite;
    Sprite imageSprite;

    float size;
    float imageSize;
    float selectedRatio;

    bool disabled;
    bool selected;

    public void Init(Sprite border, Sprite image, NodePlacementSettings placementSettings, bool disabled)
    {
        borderSprite = border;
        imageSprite = image;
        size = placementSettings.size;
        imageSize = placementSettings.selectedImageSize;
        selectedRatio = placementSettings.highlightedSizeRatio;

        this.disabled = disabled;

        CreateNode(new Color[] { });
    }

    void Update()
    {
        if (selected)
        {
            foreach (GameObject piece in framePieces)
            {
                piece.GetComponent<RectTransform>().localScale = selectedRatio * Vector2.one;
            }
            image.rectTransform.localScale = selectedRatio * Vector2.one;
            border.rectTransform.localScale = selectedRatio * Vector2.one;
        }
        else {
            foreach (GameObject piece in framePieces)
            {
                piece.GetComponent<RectTransform>().localScale = Vector2.one;
            }
            image.rectTransform.localScale = Vector2.one;
            border.rectTransform.localScale = Vector2.one;
        }
    }

    public void ChangeColor(Color[] colors) {
        DestroyCurrent();
        CreateNode(colors);
    }

    void CreateNode(Color[] colors) {
        framePieces = new List<GameObject>();
        for (int i = 0; i < colors.Length; i++) {
            GameObject framePiece = new GameObject("Frame Piece");
            RectTransform rect = framePiece.AddComponent<RectTransform>();
            rect.SetParent(transform);
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.sizeDelta = size * Vector2.one;

            Image im = framePiece.AddComponent<Image>();
            im.sprite = borderSprite;
            im.color = colors[i];
            im.type = Image.Type.Filled;
            im.fillAmount = (float)(colors.Length - i) / (float)(colors.Length);
            framePieces.Add(framePiece);
        }
        border = new GameObject("Border").AddComponent<Image>();
        border.rectTransform.SetParent(transform);
        border.rectTransform.localPosition = Vector3.zero;
        if (colors.Length > 0)
            border.rectTransform.sizeDelta = imageSize * Vector2.one;
        else
            border.rectTransform.sizeDelta = size * Vector2.one;
        border.sprite = borderSprite;
        border.color = disabled ? Color.gray : Color.white;

        image = new GameObject("Image").AddComponent<Image>();
        image.rectTransform.SetParent(transform);
        image.rectTransform.localPosition = Vector3.zero;
        if (colors.Length > 0)
            image.rectTransform.sizeDelta = imageSize * Vector2.one;
        else
            image.rectTransform.sizeDelta = size * Vector2.one;
        image.sprite = imageSprite;
        image.color = disabled ? new Color(1, 1, 1, 0.3f) : Color.white;
    }

    void DestroyCurrent() {
        if (image != null)
        {
            Destroy(image.gameObject);
        }
        if (border)
        {
            Destroy(border.gameObject);
        }
        foreach (GameObject piece in framePieces)
        {
            Destroy(piece);
        }
    }

    public void SetSelected(bool isSelected) {
        selected = isSelected;
    }
}
