using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridConfirmationCursor : MonoBehaviour
{
    CanvasGroup canvasGroup;

    RectTransform confirm;
    RectTransform reject;

    float selectedSizeRatio;

    public void Init(ConfirmationCursorSettings settings)
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Image background = new GameObject("background").AddComponent<Image>();
        background.color = new Color(0, 0, 0, .75f);
        background.transform.SetParent(transform, false);
        background.rectTransform.sizeDelta = Vector2.one * 1000;

        Image confirmBorder = new GameObject("confirm").AddComponent<Image>();
        confirmBorder.sprite = settings.borderSprite;
        confirmBorder.color = settings.confirmColour;
        confirm = confirmBorder.rectTransform;
        confirm.SetParent(transform, false);
        confirm.anchoredPosition = Vector2.up * settings.optionPosition + Vector2.left * settings.optionPosition;
        confirm.sizeDelta = Vector2.one * settings.borderSize;

        Image confirmIcon = new GameObject("icon").AddComponent<Image>();
        confirmIcon.sprite = settings.confirmSprite;
        confirmIcon.color = settings.confirmColour;
        confirmIcon.transform.SetParent(confirm, false);
        confirmIcon.rectTransform.sizeDelta = Vector2.one * settings.imageSize;

        Image rejectBorder = new GameObject("reject").AddComponent<Image>();
        rejectBorder.sprite = settings.borderSprite;
        rejectBorder.color = settings.rejectColor;
        reject = rejectBorder.rectTransform;
        reject.SetParent(transform, false);
        reject.anchoredPosition = settings.optionPosition;
        reject.sizeDelta = Vector2.one * settings.borderSize;

        Image rejectIcon = new GameObject("icon").AddComponent<Image>();
        rejectIcon.sprite = settings.rejectSprite;
        rejectIcon.color = settings.rejectColor;
        rejectIcon.transform.SetParent(reject, false);
        rejectIcon.rectTransform.sizeDelta = Vector2.one * settings.imageSize;

        selectedSizeRatio = settings.selectedSizeRatio;

        Hide();
    }

    
    public void UpdateCursor(bool confirmSelected)
    {
        Show();
        if (confirmSelected)
        {
            confirm.localScale = Vector3.one * selectedSizeRatio;
            reject.localScale = Vector3.one;
        }
        else
        {
            confirm.localScale = Vector3.one;
            reject.localScale = Vector3.one * selectedSizeRatio;
        }
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }
}
