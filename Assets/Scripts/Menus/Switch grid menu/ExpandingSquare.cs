using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandingSquare : MonoBehaviour
{
    public float minSize = 1f;
    public float maxSize = 1.6f;

    public bool selecting = false;
    public bool selected = false;

    RectTransform rt;
    Image img;
    Selectable sl;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        sl = GetComponent<Selectable>();
    }

    void Update()
    {
        if (sl != null) {
            selecting = sl.selecting;
            selected = sl.selected;
        }
    }

    void FixedUpdate()
    {
        if (selecting)
        {
            rt.localScale = rt.localScale + (new Vector3(maxSize, maxSize, 0f) - rt.localScale) / 8f;
        }
        else {
            rt.localScale = rt.localScale + (new Vector3(minSize, minSize, 0f) - rt.localScale) / 8f;
        }

        if (selected)
        {
            img.color = new Color(255, 0, 0);
        }
        else {
            img.color = new Color(255, 255, 255);
        }
    }
}
