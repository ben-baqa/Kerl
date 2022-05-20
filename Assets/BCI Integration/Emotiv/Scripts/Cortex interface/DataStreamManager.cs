using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using System.Linq;


namespace EmotivUnityPlugin
{
    public class DataStreamManager
    {
        public static DataStreamManager Instance { get; } = new DataStreamManager();
        static readonly object locker = new object();
        CortexClient ctxClient = CortexClient.Instance;
        Authorizer authorizer = Authorizer.Instance;

        Dictionary<string, DataStream> sessions;
        Dictionary<string, string> headsetToSessionID;

        public ConnectToCortexStates connectionState = ConnectToCortexStates.Service_connecting;

        public event EventHandler<string> HeadsetConnected;
        public event EventHandler<List<Headset>> QueryHeadsetOK
        {
            add { HeadsetFinder.Instance.QueryHeadsetOK += value; }
            remove { HeadsetFinder.Instance.QueryHeadsetOK -= value; }
        }

        bool debugPrint;

        private DataStreamManager()
        {
            Init();
        }

        void Init()
        {
            sessions = new Dictionary<string, DataStream>();
            headsetToSessionID = new Dictionary<string, string>();

            ctxClient.StreamDataReceived += OnStreamDataRecieved;
            ctxClient.CreateSessionOK += OnCreateSessionOk;
            ctxClient.SubscribeDataDone += OnSubscribeDataDone;

            authorizer.GetLicenseInfoDone += OnGetLicenseInfoDone;
            //ctxClient.QueryHeadsetOK += OnQueryHeadsetOK;

            //authorizer.ConnectServiceStateChanged += OnConnectionStateChanged;
        }

        private void OnStreamDataRecieved(object sender, StreamDataEventArgs e)
        {
            //Debug.Log($"DataStreamManager: Stream Data received - {e.Sid}");
            lock (locker) if (sessions.ContainsKey(e.Sid))
                    sessions[e.Sid].OnStreamDataRecieved(e);
        }

        private void OnCreateSessionOk(object sender, SessionEventArgs e)
        {
            try
            {
                string id = e.SessionId;
                ctxClient.Subscribe(authorizer.CortexToken, id, Config.dataStreams);
                lock (locker)
                {
                    sessions[id] = new DataStream(id, debugPrint);
                    headsetToSessionID[e.HeadsetId] = id;
                }

                HeadsetConnected(this, e.HeadsetId);
                Debug.Log($"Session created successfuly, new session id: {id}");
            }catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private void OnSubscribeDataDone(object sender, MultipleResultEventArgs e)
        {
            Debug.Log("DataStreamManager: SubscribeDataOK");
            foreach (JObject i in e.FailList)
                Debug.Log($"ERROR: Failed to subscribe to {i["streamName"]} stream");
            
            foreach (JObject stream in e.SuccessList)
            {
                if ((string)stream["streamName"] == "dev")
                {
                    string sid = (string)stream["sid"];
                    lock (locker) if (sessions.ContainsKey(sid))
                            sessions[sid].ConfigureDevHeaders((JArray)stream["cols"][2]);
                }
            }

            //foreach (JObject i in e.SuccessList)
            //    Debug.Log($"Subscribed to {i["streamName"]} stream");
        }


        private void OnGetLicenseInfoDone(object sender, License l)
        {
            HeadsetFinder.Instance.FinderInit();
        }

        /// <summary>
        /// Initiates the authorizer and cortex client
        /// </summary>
        public void StartAuthorize(bool debug, string license = "")
        {
            ctxClient.InitWebSocketClient();
            // Start connecting to cortex service
            authorizer.StartAction(license);

            debugPrint = debug;
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
            foreach (var item in headsetToSessionID.Where(kvp => kvp.Value == sessionID))
                headsetToSessionID.Remove(item.Key);
        }

        public void Stop()
        {
            foreach (var k in sessions)
                ctxClient.UpdateSession(authorizer.CortexToken, k.Key, "close");
            sessions.Clear();

            HeadsetFinder.Instance.StopQueryHeadset();
            ctxClient.ForceCloseWSC();
        }

        public DataStream this[string s] => sessions[headsetToSessionID[s]];
    }
}
