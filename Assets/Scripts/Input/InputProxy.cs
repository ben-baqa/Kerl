using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Provides a simple interface intended to obscure a more complex input system
/// </summary>
/// <summary>
/// Provides a simple interface intended to obscure a more complex input system
/// </summary>
public class InputProxy : MonoBehaviour
{
    public static int playerCount
    {
        get
        {
            return playerCountOverride;
        }
    }
    public static int playerCountOverride;

    public static InputProxy instance;

    public bool enableDebugInput;
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
            if (enableDebugInput && (Keyboard.current[$"numpad{n}"].IsPressed()
                || Keyboard.current[new string[] { "a", "s", "d", "f", "g", "h", "j", "k" }[n]].IsPressed()))
                return true;

            if (n >= playerCount)
                return false;
            return false;
        }
    }
    public static bool P(int n) => instance[n];

    private void Start()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        playerCountOverride = playerCountDebugOverride;
    }
}