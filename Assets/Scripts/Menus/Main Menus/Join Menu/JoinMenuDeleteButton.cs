using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

public class JoinMenuDeleteButton : MonoBehaviour
{
    int playerIndex = -1;

    public void Init(int n, UnityAction callBack)
    {
        playerIndex = n;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        button.onClick.AddListener(callBack);

        GetComponent<InputIcon>().Index = n;

        //Image image = GetComponent<Image>();
        //image.sprite = MenuSelections.GetInputSprite(n);

        //var text = GetComponentInChildren<TextMeshProUGUI>();
        //text.text = InputProxy.GetInputInfo(n).name;
    }

    void OnClick()
    {
        if (playerIndex < 0)
            return;

        InputProxy.RemoveInput(playerIndex);
    }
}
