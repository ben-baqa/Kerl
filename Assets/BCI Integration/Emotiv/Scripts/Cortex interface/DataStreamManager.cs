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
        public List<Headset> detectedHeadsets;

        public event EventHandler<HeadsetConnectEventArgs> HeadsetConnected
        {
            add { ctxClient.HeadsetConnectNotify += value; }
            remove { ctxClient.HeadsetConnectNotify -= value; }
        }

        private DataStreamManager()
        {
            sessions = new Dictionary<string, DataStream>();
            detectedHeadsets = new List<Headset>();

            ctxClient.StreamDataReceived += OnStreamDataRecieved;
            ctxClient.CreateSessionOK += OnCreateSessionOk;
            //ctxClient.GetLicenseInfoDone += OnGetLicenseInfoDone;
            //ctxClient.QueryHeadsetOK += OnQueryHeadsetOK;
            HeadsetFinder.Instance.QueryHeadsetOK += OnQueryHeadsetOK;

            authorizer.ConnectServiceStateChanged += OnConnectionStateChanged;
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

        private void OnConnectionStateChanged(object sender, ConnectToCortexStates state)
        {
            lock (locker)
            {
                connectionState = state;
                if (state == ConnectToCortexStates.Authorized)
                    HeadsetFinder.Instance.FinderInit();
            }
        }

        private void OnQueryHeadsetOK(object sender, List<Headset> headsets)
        {
            lock (locker)
            {
                detectedHeadsets.Clear();
                foreach (Headset h in headsets)
                    detectedHeadsets.Add(h);

                Debug.Log($"Headset Info Received in DataStreamManager: {headsets.Count}");
            }
        }

        private void OnGetLicenseInfoDone(object sender, License l)
        {
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
                CloseSession(k.Key);
            sessions.Clear();

            HeadsetFinder.Instance.StopQueryHeadset();
            ctxClient.ForceCloseWSC();
        }
    }
}
