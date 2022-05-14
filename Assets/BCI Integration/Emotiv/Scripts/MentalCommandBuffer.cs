using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System.Linq;
using System;

public class MentalCommandBuffer
{
    List<MentalCommand> buffer = new List<MentalCommand>();

 
    public MentalCommand[] PopData()
    {
        var data = GetData();
        Clear();
        return data;
    }
    public MentalCommand[] GetData()
    {
        MentalCommand[] data = buffer.ToArray();
        return data;
    }

    public int GetBufferSize()
    {
        return buffer.Count;
    }

    public void Clear()
    {
        buffer.Clear();
    }

    void AddToBuffer(MentalCommand command)
    {
        buffer.Add(command);
    }

    public void OnDataRecieved(object sender, ArrayList data)
    {
        double time = Convert.ToDouble(data[0]);
        string act = Convert.ToString(data[1]);
        float pow = (float)Convert.ToDouble(data[2]);
        AddToBuffer(new MentalCommand(act, time, pow));
    }
}

public struct MentalCommand
{
    public string action;
    public double timestamp;
    public float power;

    public MentalCommand(string act, double time, float pow)
    {
        action = act;
        timestamp = time;
        power = pow;
    }

    public override string ToString()
    {
        return $"Mental Command:   {action}, Power: {power}, timestamp: {timestamp}";
    }
}
