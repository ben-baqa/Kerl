using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputIcon : MonoBehaviour
{
    protected Image image;
    protected TextMeshProUGUI text;

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
                image = GetComponent<Image>();
            if (!text)
                text = GetComponentInChildren<TextMeshProUGUI>(true);

            image.sprite = MenuSelections.GetInputSprite(_index);
            image.color = MenuSelections.GetColor(_index);
            text.text = InputProxy.GetInputInfo(_index).name;
        }
    }

    int _index = -1;
}
