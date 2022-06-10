using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image bar;
    public Vector2 finalScale;
    public bool horizontal;

    RectTransform rect;

    public void Init()
    {
        rect = bar.transform as RectTransform;
        rect.sizeDelta = finalScale * (horizontal ? Vector2.up : Vector2.right);
        Deactivate();
    }

    public void Activate() => gameObject.SetActive(true);
    public void Deactivate() => gameObject.SetActive(false);

    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);

        rect.sizeDelta = finalScale * (horizontal ? Vector2.up : Vector2.right);
        rect.sizeDelta += progress * finalScale * (horizontal ? Vector2.right : Vector2.up);
    }
}
