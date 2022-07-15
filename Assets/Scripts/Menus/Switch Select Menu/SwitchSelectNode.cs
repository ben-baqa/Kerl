using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwitchSelectNode : MonoBehaviour
{
    public Vector2 Position => rectTransform.anchoredPosition;

    RectTransform rectTransform;
    float unSelectedSize;

    public void Init(LockableNode nodeInfo, float unSelectedSizeRatio)
    {
        GetComponentInChildren<TextMeshProUGUI>(true).text = nodeInfo.ItemName;
        rectTransform = GetComponent<RectTransform>();
        if (nodeInfo.locked)
        {
            gameObject.AddComponent<CanvasGroup>().alpha = 0.3f;
        }
        unSelectedSize = unSelectedSizeRatio;
        Deselect();
    }

    public void Select()
    {
        transform.localScale = Vector3.one;
    }

    public void Deselect()
    {
        transform.localScale = Vector3.one * unSelectedSize;
    }
}
