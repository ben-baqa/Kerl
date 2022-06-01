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
    public string headsetID;

    MentalCommandBuffer commandBuffer;
    MentalCommand lastCommand;

    float commandValue;
    int consecutiveComands;
    const int RAMP_COUNT = 10;
    const float INPUT_THRESHOLD = 0.5f;

    public EmotivHeadsetProxy(string id)
    {
        headsetID = id;
        //Cortex.SubscribeMentalCommands(headsetID, OnMentalCommandReceived);
        commandBuffer = new MentalCommandBuffer();
        Cortex.SubscribeMentalCommands(headsetID, commandBuffer.OnDataRecieved);
    }

    private bool ResolveInput()
    {
        return commandValue > INPUT_THRESHOLD;
        //return lastCommand.action != "neutral";
    }

    //void OnMentalCommandReceived(MentalCommand command)
    //{
    //    lastCommand = command;
    //}

    public override void Process()
    {
        foreach (MentalCommand m in commandBuffer.GetData())
        {
            float targetVal = (float)m.power;
            if (m.action == "neutral")
                targetVal = 0;
            commandValue = Mathf.Lerp(commandValue, targetVal, GetRamp(consecutiveComands / RAMP_COUNT));

            if (lastCommand.action == m.action)
                consecutiveComands++;
            else
                consecutiveComands = 0;
            lastCommand = m;
        }
    }

    // returns a sloped value that increases exponentially with n
    // as more of the same command comes in, intended to filter spikes of
    // false positives and negatives
    float GetRamp(float n)
    {
        return Mathf.Clamp01(Mathf.Pow(n, 3));
    }
}
