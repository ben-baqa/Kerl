using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputIcon : MonoBehaviour
{
    [HideInInspector]
    public Image image;
    [HideInInspector]
    public TextMeshProUGUI text;

    public RectTransform rectTransform => transform as RectTransform;

    private void Start()
    {
        image = GetComponentInChildren<Image>(true);
        text = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void Activate() => gameObject.SetActive(true);
    public void Deactivate() => gameObject.SetActive(false);

    public int Index
    {
        get
        {
            return _index;
        }
        set
        {
            _index = value;

            if (!image)
                image = GetComponentInChildren<Image>(true);
            if (!text)
                text = GetComponentInChildren<TextMeshProUGUI>(true);

            image.sprite = MenuSelections.GetInputSprite(_index);
            image.color = MenuSelections.GetColor(_index);
            text.text = InputProxy.GetInputInfo(_index).name;
        }
    }

    int _index = -1;
}
