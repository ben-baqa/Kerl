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
            if(_toggledValue)
            {
                wasReleasedSinceLastUse = false;
                _toggledValue = false;
                return true;
            }
            return false;
        }
    }

    protected bool wasReleasedSinceLastUse = false, _toggledValue;

    public virtual void Process()
    {
        if (wasReleasedSinceLastUse)
            _toggledValue = value;
        else
            wasReleasedSinceLastUse = !value;
    }

    public static implicit operator bool(InputBase i) => i.value;

    public InputType GetInputType()
    {
        if (this is KeyInput)
            return InputType.key;
        if (this is GamepadInput)
            return InputType.gamepad;
        if (this is BCIInput)
            return InputType.bci;
        if (this is NetworkInput)
            return InputType.network;
        Debug.LogWarning("Unexpected input type: " + this.GetInputType());
        return InputType.unknown;
    }
}

public class KeyInput : InputBase
{
    public override bool value => keyboard[key].IsPressed();
    private Keyboard keyboard;
    private string key;
    public KeyInput(Keyboard board, string keyCode)
    {
        keyboard = board;
        key = keyCode;
    }
}

public class GamepadInput : InputBase
{
    public override bool value => IsPressed();
    private Gamepad gamepad;
    public GamepadInput(Gamepad g) => gamepad = g;

    bool IsPressed()
    {
        return gamepad.buttonNorth.isPressed ||
            gamepad.buttonSouth.isPressed ||
            gamepad.buttonEast.isPressed ||
            gamepad.buttonWest.isPressed ||
            gamepad.leftTrigger.isPressed ||
            gamepad.rightTrigger.isPressed ||
            gamepad.leftShoulder.isPressed ||
            gamepad.rightShoulder.isPressed ||
            gamepad.leftStickButton.isPressed ||
            gamepad.rightStickButton.isPressed ||
            gamepad.dpad.IsActuated();
    }
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

    public BCIInput(HeadsetProxy headsetProxy)
    {
        headset = headsetProxy;
    }

    public override void Process()
    {
        base.Process();
        headset.Process();
    }
}

public enum InputType
{
    key, gamepad, bci, network, unknown
}