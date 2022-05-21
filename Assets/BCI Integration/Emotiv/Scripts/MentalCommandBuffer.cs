using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System.Linq;
using System;

public class MentalCommandBuffer
{
    Queue<MentalCommand> buffer = new Queue<MentalCommand>();

    public MentalCommand[] GetData()
    {
        MentalCommand[] data = buffer.ToArray();
        Clear();
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
        buffer.Enqueue(command);

        while (buffer.Count > Config.MENTAL_COMMAND_BUFFER_SIZE)
            buffer.Dequeue();
    }

    public void OnDataRecieved(object sender, ArrayList data)
    {
        double time = Convert.ToDouble(data[0]);
        string act = Convert.ToString(data[1]);
        float pow = (float)Convert.ToDouble(data[2]);
        AddToBuffer(new MentalCommand(time, act, pow));
    }
    public void OnDataRecieved(object sender, MentalCommand e)
    {
        AddToBuffer(e);
    }
    public void OnDataRecieved(MentalCommand e)
    {
        AddToBuffer(e);
    }
}
