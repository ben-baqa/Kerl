using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionToken
{
    public Vector3 target;
    public Sprite Sprite { set { image.sprite = value; } }
    public Color Colour { set { image.color = value; } }

    RectTransform rect;
    Image image;

    public SelectionToken(Transform parent, float size)
    {
        GameObject tokenInstance = new GameObject("token");
        tokenInstance.transform.SetParent(parent, false);
        image = tokenInstance.AddComponent<Image>();
        rect = image.rectTransform;
        rect.sizeDelta = Vector2.one * size;
        rect.localScale = Vector3.one;
        image.enabled = false;
    }

    public void Enable(Vector3 position)
    {
        image.enabled = true;
        target = position;
        rect.anchoredPosition = position;
    }

    public void Process()
    {
        rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, target, 0.2f);
    }
}

[System.Serializable]
public class SelectionTokenSettings
{
    public Vector2 neutralPosition;
    public Vector2 selectedPosition;
    public Vector2 neutralSpacing;
    public Vector2 selectedSpacing;
    public float spacing;
    public float size;

    public Sprite onSprite;
    public Sprite offSprite;
}