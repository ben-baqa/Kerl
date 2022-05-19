using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace EmotivUnityPlugin
{
    public class DataStreamManager
    {
        public static DataStreamManager Instance { get; } = new DataStreamManager();
        static readonly object locker = new object();
        CortexClient ctxClient = CortexClient.Instance;
        Authorizer authorizer = Authorizer.Instance;

        Dictionary<string, DataStream> sessions;

        public ConnectToCortexStates connectionState = ConnectToCortexStates.Service_connecting;

        public event EventHandler<HeadsetConnectEventArgs> HeadsetConnected
        {
            add { ctxClient.HeadsetConnectNotify += value; }
            remove { ctxClient.HeadsetConnectNotify -= value; }
        }
        public event EventHandler<List<Headset>> QueryHeadsetOK
        {
            add { HeadsetFinder.Instance.QueryHeadsetOK += value; }
            remove { HeadsetFinder.Instance.QueryHeadsetOK -= value; }
        }

        //public List<Headset> headsets;
        //public Event UpdateHeadsetList;

        private DataStreamManager()
        {
            Init();
        }

        void Init()
        {
            sessions = new Dictionary<string, DataStream>();

            ctxClient.StreamDataReceived += OnStreamDataRecieved;
            ctxClient.CreateSessionOK += OnCreateSessionOk;
            authorizer.GetLicenseInfoDone += OnGetLicenseInfoDone;
            //ctxClient.QueryHeadsetOK += OnQueryHeadsetOK;
            //HeadsetFinder.Instance.QueryHeadsetOK += OnQueryHeadsetOK;

            //authorizer.ConnectServiceStateChanged += OnConnectionStateChanged;
        }

        private void OnStreamDataRecieved(object sender, StreamDataEventArgs e)
        {
            lock (locker)
            {
                if (sessions.ContainsKey(e.Sid))
                    sessions[e.Sid].OnStreamDataRecieved(e);
            }
        }

        private void OnCreateSessionOk(object sender, SessionEventArgs e)
        {
            lock (locker)
            {
                string id = e.SessionId;
                Debug.Log($"Session created successfuly, new session id: {id}");
                sessions.Add(id, new DataStream(id));
                ctxClient.Subscribe(authorizer.CortexToken, id, Config.dataStreams);
            }
        }

        //private void OnConnectionStateChanged(object sender, ConnectToCortexStates state)
        //{
        //    lock (locker)
        //    {
        //        connectionState = state;
        //        if (state == ConnectToCortexStates.Authorized)
        //            HeadsetFinder.Instance.FinderInit();
        //    }
        //}

        private void OnGetLicenseInfoDone(object sender, License l)
        {
            Debug.Log("This should be gettig called");
            HeadsetFinder.Instance.FinderInit();
        }

        /// <summary>
        /// Initiates the authorizer and cortex client
        /// </summary>
        public void StartAuthorize(string license = "")
        {
            ctxClient.InitWebSocketClient();
            // Start connecting to cortex service
            authorizer.StartAction(license);
        }

        /// <summary>
        /// Starts a cortex session with a given headset,
        /// triggers a callback that will also subscribe to data streams
        /// </summary>
        public void StartSession(string headsetID)
        {
            Debug.Log($"Attempting to start session with headset: {headsetID}");
            ctxClient.CreateSession(authorizer.CortexToken, headsetID, "open");
        }

        public void CloseSession(string sessionID)
        {
            ctxClient.UpdateSession(authorizer.CortexToken, sessionID, "close");
            sessions.Remove(sessionID);
        }

        public void Stop()
        {
            foreach (var k in sessions)
                ctxClient.UpdateSession(authorizer.CortexToken, k.Key, "close");
            sessions.Clear();

            HeadsetFinder.Instance.StopQueryHeadset();
            ctxClient.ForceCloseWSC();
        }
    }
}
