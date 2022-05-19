using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmotivUnityPlugin
{
    public class DataStream
    {
        public event EventHandler<MentalCommandEventArgs> MentalCommandReceived;
        public event EventHandler<SysEventArgs> SysEventReceived;
        public event EventHandler<ArrayList> DevDataReceived;

        //public event EventHandler
        // TODO
        // add headset connection event
        // add contact quality filtered by Headset nodes

        public MentalCommandBuffer mentalCommands;

        string sessionID;

        public DataStream(string id)
        {
            mentalCommands = new MentalCommandBuffer();
            sessionID = id;
        }

        public void OnStreamDataRecieved(StreamDataEventArgs e)
        {
            Debug.Log($"Stream Data recieved: {e.StreamName}");
            ArrayList data = e.Data;
            double time = Convert.ToDouble(data[0]);
            switch (e.StreamName)
            {
                case DataStreamName.MentalCommands:
                    string act = Convert.ToString(data[1]);
                    double pow = Convert.ToDouble(data[2]);
                    MentalCommandEventArgs comEvent = new MentalCommandEventArgs(time, act, pow);

                    MentalCommandReceived(this, comEvent);
                    mentalCommands.OnDataRecieved(comEvent);
                    break;

                case DataStreamName.SysEvents:
                    string detection = Convert.ToString(data[1]);
                    string eventMsg = Convert.ToString(data[2]);
                    SysEventArgs sysEvent = new SysEventArgs(time, detection, eventMsg);

                    SysEventReceived(this, sysEvent);
                    break;

                case DataStreamName.DevInfos:
                    DevDataReceived(this, data);
                    break;

                default:
                    Debug.Log($"unused stream data on {e.StreamName}");
                    break;
            }
        }

        public void Close()
        {
            DataStreamManager.Instance.CloseSession(sessionID);
        }
    }
}