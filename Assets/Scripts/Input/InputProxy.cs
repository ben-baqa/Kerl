using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Provides a simple interface intended to obscure a more complex input system
/// </summary>
public class InputProxy : MonoBehaviour
{
    public int playerCount => inputs.Count;

    private List<InputBase> inputs;

    public bool enableDebugInput;


    public bool this[int n]
    {
        get
        {
            if (enableDebugInput && Keyboard.current[$"numpad{n}Key"].IsPressed())
                return true;

            if (n >= playerCount)
                return false;
            return inputs[n];
        }
    }

    private void Update()
    {
        foreach (var i in inputs)
            i.Process();
    }

    public void AddInput(InputBase input)
    {
        inputs.Add(input);
    }
    public void RemoveInput(InputBase input)
    {
        inputs.Remove(input);
    }
    public void RemoveInput(int index)
    {
        inputs.RemoveAt(index);
    }

    public void AddNetworkInput(string id)
    {
        inputs.Add(new NetworkInput(id));
        //NetworkMessageHandler.Instance.InputRecieved += NetworkInput.NetworkUpdate;
    }
}
