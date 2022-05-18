using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Provides a simple interface to different input types
/// </summary>
public abstract class InputBase
{
    /// <summary>
    /// True if the input is currently pressed
    /// </summary>
    public virtual bool value => false;
    /// <summary>
    /// Input will be consumed on use until released and pressed again
    /// </summary>
    public virtual bool toggledValue
    {
        get
        {
            toggle = false;
            if (heldValue)
            {
                heldValue = false;
                return true;
            }
            return false;
        }
    }

    protected bool toggle, heldValue;

    public virtual void Process()
    {
        if (toggle)
            heldValue = value;
        else
            toggle |= value;
    }

    public static implicit operator bool(InputBase i) => i.value;
}

public class KeyInput : InputBase
{
    public override bool value => keyboard[key].isPressed;
    private Keyboard keyboard;
    private Key key;
    public KeyInput(Keyboard board, Key keyCode)
    {
        keyboard = board;
        key = keyCode;
    }
}

public class GamepadInput : InputBase
{
    public override bool value => gamepad.IsPressed();
    private Gamepad gamepad;
    public GamepadInput(Gamepad g) => gamepad = g;
}

public class NetworkInput : InputBase
{
    public override bool value => networkValue;

    private bool networkValue;
    // provides player identifier to filter input messages - NOT IMPLEMENTED
    private string networkID;

    public NetworkInput(string NetworkID) => networkID = NetworkID;
    public void NetworkUpdate(object sender, NetworkInputEventArgs args) {if(args.id == networkID) networkValue = args.value;}
}

public class BCIInput : InputBase
{
    public override bool value => headset.value;

    HeadsetProxy headset;

    public override void Process()
    {
        base.Process();
        headset.Process();
    }
}