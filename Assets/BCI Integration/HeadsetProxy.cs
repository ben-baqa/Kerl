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
    public string identifier;

    // called by Update through input proxy
    public virtual void Process()
    {

    }

    public virtual void OnDelete() { }
}

public class EmotivHeadsetProxy : HeadsetProxy
{
    public override bool value => ResolveInput();
    public string headsetID;

    MentalCommand lastCommand;

    float commandValue;
    int consecutiveComands;
    const int RampCount = 10;
    const float InputThreshold = 0.5f;

    public EmotivHeadsetProxy(string id, string profilename)
    {
        headsetID = id;
        identifier = profilename;
        Cortex.SubscribeMentalCommands(headsetID, OnMentalCommandReceived);
    }

    public override void OnDelete()
    {
        Cortex.profiles.UnloadProfile(identifier, headsetID);
        Cortex.EndSessionByHeadset(headsetID);
    }

    private bool ResolveInput()
    {
        return commandValue > InputThreshold;
        //return lastCommand.action != "neutral";
    }

    public void OnMentalCommandReceived(MentalCommand command)
    {
        float targetVal = (float)command.power;
        if (command.action == "neutral")
            targetVal = 0;
        commandValue = Mathf.Lerp(commandValue, targetVal, GetRamp(consecutiveComands / RampCount));

        if (lastCommand && lastCommand.action == command.action)
            consecutiveComands++;
        else
            consecutiveComands = 0;
        lastCommand = command;
    }

    // returns a sloped value that increases exponentially with n
    // as more of the same command comes in, intended to filter spikes of
    // false positives and negatives
    float GetRamp(float n)
    {
        return Mathf.Clamp01(Mathf.Pow(n, 3));
    }
}
