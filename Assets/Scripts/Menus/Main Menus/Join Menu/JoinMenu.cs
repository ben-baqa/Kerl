using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Runs the join menu on the title screen
/// </summary>
public class JoinMenu : MonoBehaviour
{
    public static int player_count => InputProxy.playerCount;

    public JoinButton[] player_buttons;
    public TextMeshProUGUI timerText;

    public int maxPlayerCount = 4;
    public float maxTimer;

    AudioSource sfx;
    List<InputControl> activeKeyInputs = new List<InputControl>();
    List<Gamepad> activeGamepadInputs = new List<Gamepad>();

    float timer;


    void Start()
    {
        sfx = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        ResetMenu();

        InputProxy.InputRemoved += OnInputRemoved;
    }
    private void OnDisable()
    {
        InputProxy.InputRemoved -= OnInputRemoved;
    }

    void Update()
    {
        CheckForNewInput();

        if (AllPlayersReady())
        {
            timer -= Time.deltaTime;
            timerText.text = "" + ((int)timer + 1);

            if (timer <= 0)
                // continue to next menu
                FindObjectOfType<MainMenu>().Continue();
            else if (timer <= 3.9f && !sfx.isPlaying)
                sfx.Play();
        }
        else if (timer != maxTimer)
        {
            timer = maxTimer;
            timerText.text = "";
        }
    }

    void ResetMenu()
    {
        foreach (var b in player_buttons)
            b.Reset(MenuSelections.GetInputSprite(-1));

        for (int i = 0; i < player_count; i++)
            player_buttons[i].Join(i);

        timer = 0;

        // fill active inputs with extant controls
        List<InputControl> activeKeyInputs = new List<InputControl>();
        List<Gamepad> activeGamepadInputs = new List<Gamepad>();
        foreach(InputInfo i in InputProxy.GetAllInputInfo())
        {
            if (i.type == InputType.key)
                activeKeyInputs.Add(i.device[i.name]);
            else if (i.type == InputType.gamepad)
                activeGamepadInputs.Add(i.device as Gamepad);
        }
    }

    void CheckForNewInput()
    {
        foreach(InputDevice device in InputSystem.devices)
        {
            if (device.IsActuated())
                if (device is Gamepad)
                {
                    if (!activeGamepadInputs.Contains(device as Gamepad))
                        foreach (InputControl control in device.allControls)
                            if (control.IsPressed())
                            {
                                AddGamepadInput(device as Gamepad);
                                break;
                            }
                }
                else if (device is Keyboard)
                {
                    foreach (InputControl control in device.allControls)
                        if (control.IsPressed() && !activeKeyInputs.Contains(control))
                            AddKeyboardInput(control);
                }
                else if (!(device is Mouse))
                    Debug.LogWarning("Unhandled / unrecognized input device: " + device.displayName);
        }
    }

    void AddGamepadInput(Gamepad gamepad)
    {
        if (player_count >= maxPlayerCount)
            return;

        activeGamepadInputs.Add(gamepad);

        InputProxy.AddInput(new GamepadInput(gamepad));
        player_buttons[player_count - 1].Join(player_count - 1);
    }

    void AddKeyboardInput(InputControl control)
    {
        if (player_count >= maxPlayerCount)
            return;

        activeKeyInputs.Add(control);
        if (control.name == "anyKey")
            return;

        InputProxy.AddInput(new KeyInput(control.device as Keyboard, control.name));
        player_buttons[player_count - 1].Join(player_count - 1);
    }

    bool AllPlayersReady()
    {
        foreach (JoinButton j in player_buttons)
            if (!j.ready)
                return false;
        return player_count > 0;
    }

    void OnInputRemoved(InputInfo info)
    {
        InputControl keyToRemove = null;
        Gamepad gamepadToRemove = null;
        if (info.type == InputType.key)
        {
            foreach (InputControl i in activeKeyInputs)
                if (i.name == info.name)
                    keyToRemove = i;
        }
        else if (info.type == InputType.gamepad)
        {
            foreach (Gamepad g in activeGamepadInputs)
                if (g == info.device)
                    gamepadToRemove = g;
        }

        if (keyToRemove != null)
            activeKeyInputs.Remove(keyToRemove);
        if (gamepadToRemove != null)
            activeGamepadInputs.Remove(gamepadToRemove);

        ResetMenu();
    }
}
