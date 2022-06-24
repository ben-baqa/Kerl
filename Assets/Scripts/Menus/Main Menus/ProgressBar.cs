using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image bar;
    public Vector2 finalScale;
    public bool horizontal;
    public float lerp = 0.2f;

    RectTransform rect;

    float target;
    float progress;

    public void Init()
    {
        rect = bar.transform as RectTransform;
        rect.sizeDelta = finalScale * (horizontal ? Vector2.up : Vector2.right);
        Deactivate();
    }

    private void FixedUpdate()
    {
        if (!rect)
        {
            Init();
            return;
        }

        progress = Mathf.Lerp(progress, target, lerp);

        rect.sizeDelta = finalScale * (horizontal ? Vector2.up : Vector2.right);
        rect.sizeDelta += progress * finalScale * (horizontal ? Vector2.right : Vector2.up);
    }

    public void Activate() => gameObject.SetActive(true);
    public void Deactivate() => gameObject.SetActive(false);

    public void SetProgress(float f)
    {
        target = Mathf.Clamp01(f);
    }
}
