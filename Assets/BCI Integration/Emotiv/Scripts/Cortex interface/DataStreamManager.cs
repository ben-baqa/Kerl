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

        /// <summary>
        /// Active DataStreams from all open sessions
        /// </summary>
        Dictionary<string, DataStream> sessions;
        /// <summary>
        /// Maps each open session ID to the respective headset, used to enable subscriptions by headset ID
        /// </summary>
        Dictionary<string, string> headsetToSessionID;

        
        public ConnectToCortexStates connectionState = ConnectToCortexStates.Service_connecting;

        /// <summary>
        /// Sent out when a new headset has been connected
        /// </summary>
        public event EventHandler<string> HeadsetConnected;
        /// <summary>
        /// Sent out when a new list of available headsets has been found
        /// </summary>
        public event EventHandler<List<Headset>> QueryHeadsetOK
        {
            add { HeadsetFinder.Instance.QueryHeadsetOK += value; }
            remove { HeadsetFinder.Instance.QueryHeadsetOK -= value; }
        }

        // verbose logs
        bool debugPrint;

        private DataStreamManager()
        {
            Init();
        }

        void Init()
        {
            sessions = new Dictionary<string, DataStream>();
            headsetToSessionID = new Dictionary<string, string>();

            // Connect event handlers
            ctxClient.StreamDataReceived += OnStreamDataRecieved;
            ctxClient.CreateSessionOK += OnCreateSessionOk;
            ctxClient.SubscribeDataDone += OnSubscribeDataDone;
            ctxClient.HeadsetConnectNotify +=
                (object sender, HeadsetConnectEventArgs e) =>
                {
                    Debug.Log($"Headset paired: {e.HeadsetId}");
                    ctxClient.QueryHeadsets("");
                };

            authorizer.GetLicenseInfoDone += OnGetLicenseInfoDone;
        }

        /// <summary>
        /// Routes incoming stream data from CortexClient to the relevant DataStream(s)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStreamDataRecieved(object sender, StreamDataEventArgs e)
        {
            //Debug.Log($"DataStreamManager: Stream Data received - {e.Sid}");
            lock (locker) if (sessions.ContainsKey(e.Sid))
                    sessions[e.Sid].OnStreamDataRecieved(e);
        }

        /// <summary>
        /// Called by Cortexclient when a session has been successfully created,
        /// creates a DataStream object to handle incoming data
        /// </summary>
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

        /// <summary>
        /// Called by CortexClient when a session has successfully subscribed to data,
        /// prepares any relevant DataStream(s) for incoming data
        /// </summary>
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
        }

        /// <summary>
        /// Called by CortexClient when Authorization is complete, begin looking for available headsets
        /// </summary>
        private void OnGetLicenseInfoDone(object sender, License l)
        {
            HeadsetFinder.Instance.FinderInit();
        }

        /// <summary>
        /// Initiates the authorizer and cortex client,
        /// called externally at the beginning of the program
        /// </summary>
        /// <param name="debug">enable verbose logs</param>
        /// <param name="license">uneccessary in most cases,
        /// if you need this you probably know what you are doing and will be changing this code anyways</param>
        public void Start(bool debug, string license = "")
        {
            ctxClient.InitWebSocketClient();
            // Start connecting to cortex service
            authorizer.StartAction(license);

            debugPrint = debug;
        }

        /// <summary>
        /// Stops the connection to the Cortex service,
        /// called externally (preferably in OnApplicationQuit)
        /// </summary>
        public void Stop()
        {
            foreach (var k in sessions)
                ctxClient.UpdateSession(authorizer.CortexToken, k.Key, "close");
            sessions.Clear();
            headsetToSessionID.Clear();

            HeadsetFinder.Instance.StopQueryHeadset();
            ctxClient.ForceCloseWSC();
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
        /// <summary>
        /// Closes an individual session
        /// </summary>
        /// <param name="sessionID">the session to close</param>
        public void CloseSession(string sessionID)
        {
            ctxClient.UpdateSession(authorizer.CortexToken, sessionID, "close");
            sessions.Remove(sessionID);
            foreach (var item in headsetToSessionID.Where(kvp => kvp.Value == sessionID))
                headsetToSessionID.Remove(item.Key);
        }

        /// <summary>
        /// Connect a Device that is discovered, but unavailable
        /// </summary>
        public void ConnectDevice(string headsetID)
        {
            ctxClient.ConnectDevice(headsetID);
        }

        /// <summary>
        /// Connects the provided callback function to the typed
        /// data stream of the given headset, provided it exists.
        /// </summary>
        /// <typeparam name="T">The type of data to subscribe to</typeparam>
        /// <param name="headsetID">ID of the desired headset stream</param>
        /// <param name="callBack">Function to be called</param>
        /// <returns>truew if successful</returns>
        public bool SubscribeTo<T>(string headsetID, Action<T> callBack) where T : EventArgs
        {
            if (!headsetToSessionID.ContainsKey(headsetID))
            {
                Debug.LogWarning("Attempted to Subscribe to a headset stream that doesn't exist");
                return false;
            }

            try
            {
                DataStream dataStream = sessions[headsetToSessionID[headsetID]];
                switch (typeof(T))
                {
                    case Type mType when mType == typeof(MentalCommand):
                        dataStream.MentalCommandReceived +=
                            (object sender, MentalCommand data) => callBack(data as T);
                        break;
                    case Type dType when dType == typeof(DevData):
                        dataStream.DevDataReceived +=
                            (object sender, DevData data) => callBack(data as T);
                        break;
                    case Type sType when sType == typeof(SysEventArgs):
                        dataStream.SysEventReceived +=
                            (object sender, SysEventArgs data) => callBack(data as T);
                        break;
                    default:
                        Debug.LogWarning($"Attempted to subscribe to unsupported data stream: {typeof(T)}");
                        return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
        /// <summary>
        /// Disconnects the provided callback function from the typed
        /// data stream of the given headset, provided it exists
        /// </summary>
        /// <typeparam name="T">The type of data to subscribe to</typeparam>
        /// <param name="headsetID">ID of the desired headset stream</param>
        /// <param name="callBack">Function to be removed</param>
        /// <returns>true if successful</returns>
        public bool Unsubscribe<T>(string headsetID, Action<T> callBack) where T : EventArgs
        {
            if (!headsetToSessionID.ContainsKey(headsetID))
            {
                if (debugPrint)
                    Debug.LogWarning("Attempted to Unsubscribe from a headset stream that doesn't exist");
                return false;
            }

            try
            {
                DataStream dataStream = sessions[headsetToSessionID[headsetID]];
                switch (typeof(T))
                {
                    case Type mType when mType == typeof(MentalCommand):
                        dataStream.MentalCommandReceived -=
                            (object sender, MentalCommand data) => callBack(data as T);
                        break;
                    case Type dType when dType == typeof(DevData):
                        dataStream.DevDataReceived -=
                            (object sender, DevData data) => callBack(data as T);
                        break;
                    case Type sType when sType == typeof(SysEventArgs):
                        dataStream.SysEventReceived -=
                            (object sender, SysEventArgs data) => callBack(data as T);
                        break;
                    default:
                        if (debugPrint)
                            Debug.LogWarning($"Attempted to Unsibscribe from unsupported data stream: {typeof(T)}");
                        return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}
