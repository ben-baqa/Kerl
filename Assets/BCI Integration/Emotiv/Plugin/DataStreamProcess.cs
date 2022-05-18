using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmotivUnityPlugin
{
    public class DataStreamProcess
    {
        static readonly object locker = new object();

        CortexClient ctxClient = CortexClient.Instance;
        HeadsetFinder headsetFinder = HeadsetFinder.Instance;

        public ConnectToCortexStates connectionState = ConnectToCortexStates.Service_connecting;

        public event EventHandler<ArrayList> DevDataReceived;       // contact quality
        public event EventHandler<ArrayList> MentalCommandRecieved; // mental command
        public event EventHandler<ArrayList> SysEventRecieved;      // training events

        public event EventHandler<Dictionary<string, JArray>> SubscribedOK;
        public event EventHandler<DateTime> LicenseExpired;             // inform license expired
        public event EventHandler<DateTime> LicenseValidTo;             // inform license valid to date

        // notify headset connecting status
        public event EventHandler<HeadsetConnectEventArgs> HeadsetConnectNotify
        {
            add { ctxClient.HeadsetConnectNotify += value; }
            remove { ctxClient.HeadsetConnectNotify -= value; }
        }
        public event EventHandler<List<string>> StreamStopNotify;
        public event EventHandler<string> SessionClosedNotify;

        public event EventHandler<SessionEventArgs> SessionActivedOK
        {
            add { sessionHandler.SessionActived += value; }
            remove { sessionHandler.SessionActived -= value; }
        }
        public event EventHandler<string> CreateSessionFail;

        public event EventHandler<List<Headset>> QueryHeadsetOK
        {
            add { headsetFinder.QueryHeadsetOK += value; }
            remove { headsetFinder.QueryHeadsetOK -= value; }
        }
        public event EventHandler<string> UserLogoutNotify;             // inform license valid to date

        // For test
        public event EventHandler<string> ErrorNotify;

        public event EventHandler<bool> BTLEPermissionGrantedNotify
        {
            add { ctxClient.BTLEPermissionGrantedNotify += value; }
            remove { ctxClient.BTLEPermissionGrantedNotify -= value; }
        }

    }
}