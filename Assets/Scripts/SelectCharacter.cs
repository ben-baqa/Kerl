using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour
{
    enum PlayerState
    {
        NOT_JOINED,
        JOINED,
        READY
    }

    public Text[] player_text;

    public float max_timer;

    private InputProxy inputProxy;
    private PlayerState[] player_state;

    private int player_count;
    private int player_ready;

    private float countdown_timer;

    private bool[] last_input_state;
    void Start()
    {
        player_state = new PlayerState[] { PlayerState.NOT_JOINED, PlayerState.NOT_JOINED, PlayerState.NOT_JOINED, PlayerState.NOT_JOINED };
        last_input_state = new bool[] { false, false, false, false };
        inputProxy = FindObjectOfType<InputProxy>();
    }

    void Update()
    {
        if (last_input_state[0] == inputProxy.p1)
        {
            UpdateState(0);
        }
        if (last_input_state[0] == inputProxy.p2)
        {
            UpdateState(1);
        }
        if (last_input_state[0] == inputProxy.p3)
        {
            UpdateState(2);
        }
        if (last_input_state[0] == inputProxy.p4)
        {
            UpdateState(3);
        }
        UpdateNumbers();
        if (player_count == player_ready)
        {
            if (countdown_timer <= 0)
            {
                // next scene
            }
            else
            {
                countdown_timer -= Time.deltaTime;
            }
        }
        else if (countdown_timer != max_timer)
        {
            countdown_timer = max_timer;
        }
    }

    void UpdateText()
    {
        for (int i = 0; i < 4; i++)
        {
            if (player_state[i] == PlayerState.NOT_JOINED)
            {
                player_text[i].text = "Player " + i + " [N]";
            }
            else if (player_state[i] == PlayerState.JOINED)
            {
                player_text[i].text = "Player " + i + " [J]";
            }
            else if (player_state[i] == PlayerState.READY)
            {
                player_text[i].text = "Player " + i + " [R]";
            }
        }
    }

    void UpdateState(int player)
    {
        if (player_state[player] == PlayerState.NOT_JOINED)
        {
            player_state[player] = PlayerState.JOINED;
        }
        else if (player_state[player] == PlayerState.JOINED)
        {
            player_state[player] = PlayerState.READY;
        }
    }

    void UpdateNumbers()
    {
        player_count = 0;
        player_ready = 0;
        for (int i = 0; i < 4; i++)
        {
            if (player_state[i] > 0)
            {
                player_count++;
                if (player_state[i] == PlayerState.READY)
                {
                    player_ready++;
                }
            }
        }
    }
}
