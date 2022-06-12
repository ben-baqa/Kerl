using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using EmotivUnityPlugin;

/// <summary>
/// Provides a simple interface intended to obscure a more complex input system
/// </summary>
public class InputProxy : MonoBehaviour
{
    public static int playerCount
    {
        get
        {
            if (playerCountOverride > 0)
                return playerCountOverride;
            return inputs.Count;
        }
    }
    public static bool any
    {
        get
        {
            for (int i = 0; i < playerCount; i++)
                if (P(i))
                    return true;
            return false;
        }
    }
    public static int playerCountOverride;
    private static List<InputBase> inputs = new List<InputBase>();
    public static List<InputInfo> inputInfo = new List<InputInfo>();

    public static InputProxy instance;

    public static EventBuffer<InputInfo> InputAdded = new EventBuffer<InputInfo>();
    public static EventBuffer<InputInfo> InputRemoved = new EventBuffer<InputInfo>();

    public static bool enableDebugInput;
    public bool debugInput = true;
    public int playerCountDebugOverride = 0;

    public bool p1 => instance[0];
    public bool p2 => instance[1];
    public bool p3 => instance[2];
    public bool p4 => instance[3];
    public bool p5 => instance[4];
    public bool p6 => instance[5];
    public bool p7 => instance[6];
    public bool p8 => instance[7];

    public bool this[int n]
    {
        get
        {
            if (enableDebugInput && (Keyboard.current[$"numpad{n}Key"].IsPressed()
                || Keyboard.current[$"digit{n}Key"].IsPressed()))
                return true;

            if (n >= inputs.Count || n < 0)
                return false;
            return inputs[n];
        }
    }
    public static bool P(int n) => instance[n];
    public static bool GetInput(int n) => instance[n];
    public static bool GetToggledInput(int n)
    {
        if (enableDebugInput && Keyboard.current[Key.Numpad0 + n].wasPressedThisFrame)
            return true;

        if (n >= inputs.Count || n < 0)
            return false;
        return inputs[n].toggledValue;
    }

    private void Start()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        playerCountOverride = playerCountDebugOverride;
        enableDebugInput = debugInput;
    }

    private void Update()
    {
        foreach (var i in inputs)
            i.Process();

        InputAdded.Process();
        InputRemoved.Process();
    }

    public static void AddInput(InputBase input)
    {
        inputs.Add(input);
        InputAdded.OnParentEvent(null, new InputInfo(input, inputs.Count));
    }
    public static void RemoveInput(InputBase input)
    {
        InputInfo eventInfo = new InputInfo("null");
        for (int i = 0; i < inputs.Count; i++)
            if (inputs[i] == input)
                eventInfo = GetInputInfo(i);
        inputs.Remove(input);
        InputRemoved.OnParentEvent(null, eventInfo);
        
    }
    public static void RemoveInput(int index)
    {;
        InputInfo eventInfo = new InputInfo(inputs[index], index);
        inputs.RemoveAt(index);
        InputRemoved.OnParentEvent(null, eventInfo);
    }

    public static void AddNetworkInput(string id)
    {
        AddInput(new NetworkInput(id));
        //NetworkMessageHandler.Instance.InputRecieved += NetworkInput.NetworkUpdate;
    }

    public static InputInfo GetInputInfo(int index)
    {
        if (index >= 0 && index < inputs.Count)
            return new InputInfo(inputs[index], index);

        //Debug.LogWarning("Attempting to access info on an input that doesn't exist");
        return new InputInfo("Invalid input index");
    }
}
