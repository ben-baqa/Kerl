using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

/// <summary>
/// interface of different flavours if BCI input
/// </summary>
public class HeadsetProxy
{
    public virtual bool value => false;

    // called by Update through input proxy
    public virtual void Process()
    {

    }
}

public class EmotivHeadsetProxy : HeadsetProxy
{
    public override bool value => ResolveInput();
    public Headset headset;

    private MentalCommand lastCommand;

    EmotivHeadsetProxy(Headset h)
    {
        headset = h;
    }

    public override void Process()
    {
        // update dev stream; contact quality, battery, etc.
        
    }

    private bool ResolveInput()
    {
        return lastCommand.action != "neutral";
    }
}
