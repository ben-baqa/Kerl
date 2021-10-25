using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectCharacter : MonoBehaviour
{
    enum PlayerState
    {
        NOT_JOINED,
        JOINED,
        READY
    }

    public JoinButton[] player_buttons;
    public Text timer;

    public float max_timer;

    private InputProxy inputProxy;
    private AudioSource sfx;
    private PlayerState[] player_state;

    public static int player_count;
    private int player_ready;

    private float countdown_timer;

    private bool[] last_input_state;
    void Start()
    {
        player_state = new PlayerState[] { PlayerState.NOT_JOINED, PlayerState.NOT_JOINED, PlayerState.NOT_JOINED, PlayerState.NOT_JOINED };
        last_input_state = new bool[] { true, true, true, true };
        inputProxy = FindObjectOfType<InputProxy>();
        sfx = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (inputProxy.p1)
        {
            if (last_input_state[0])
                UpdateState(0);
        }
        else
            last_input_state[0] = true;

        if (inputProxy.p2)
        {
            if (last_input_state[1])
                UpdateState(1);
        }
        else
            last_input_state[1] = true;

        if (inputProxy.p3)
        {
            if (last_input_state[2])
                UpdateState(2);
        }
        else
            last_input_state[2] = true;

        if (inputProxy.p4)
        {
            if (last_input_state[3])
                UpdateState(3);
        }
        else
            last_input_state[3] = true;


        UpdateNumbers();
        if (player_count == player_ready && player_count > 0)
        {
            if (countdown_timer <= 0)
            {
                // next scene
                SceneManager.LoadScene("Game");
            }
            else if (countdown_timer <= 3.9f && !sfx.isPlaying)
                sfx.Play();
            else
            {
                countdown_timer -= Time.deltaTime;
                timer.text = "" + (int)countdown_timer;
            }
        }
        else if (countdown_timer != max_timer)
        {
            countdown_timer = max_timer;
            timer.text = "";
        }
    }

    void UpdateState(int player)
    {
        if (player_state[player] == PlayerState.NOT_JOINED)
        {
            // sound here
            player_state[player] = PlayerState.JOINED;
            last_input_state[player] = false;
            player_buttons[player].Join();
        }
        else if (player_state[player] == PlayerState.JOINED)
        {
            // sound here
            player_state[player] = PlayerState.READY;
            player_buttons[player].Ready();
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
