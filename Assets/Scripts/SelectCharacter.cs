using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour
{
    public Text[] player_text;
    private InputProxy inputProxy;
    private bool[] player_selected;
    void Start()
    {
        player_selected = new bool[] { false, false, false, false };
        inputProxy = FindObjectOfType<InputProxy>();
    }

    void Update()
    {
        UpdateText();
        player_selected[0] = inputProxy.p1;
        player_selected[1] = inputProxy.p2;
        player_selected[2] = inputProxy.p3;
        player_selected[3] = inputProxy.p4;
    }

    void UpdateText()
    {
        for (int i = 0; i < 4; i++) { 
            if (player_selected[i])
            {
                player_text[i].text = "[Player " + i + "]";
            }
            else
            {
                player_text[i].text = "Player " + i;
            }
        }
    }
}
