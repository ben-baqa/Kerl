using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using System.Linq;


namespace EmotivUnityPlugin
{
    /// <summary>
    /// Manages the creation and data subcription of mutliple headset sessios at once
    /// </summary>
    public class DataStreamManager
    {
        public static DataStreamManager Instance { get; } = new DataStreamManager();
        static readonly object locker = new object();
        CortexClient ctxClient = CortexClient.Instance;
        Authorizer authorizer = Authorizer.Instance;
        DataSubscriber dataSubscriber = DataSubscriber.Instance;

        /// <summary>
        /// Active DataStreams from all open sessions
        /// </summary>
        Dictionary<string, DataStream> sessions;
        /// <summary>
        /// Maps each open session ID to the respective headset, used to enable subscriptions by headset ID
        /// </summary>
        Dictionary<string, string> headsetToSessionID;
        /// <summary>
        /// set when creating a session, used to send our headsetConnected event when first data comes in
        /// </summary>
        string connectingHeadset = null;

        
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

        public void Init()
        {
            dataSubscriber = DataSubscriber.Instance;
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
            if (connectingHeadset != null)
            {
                foreach (var kvp in headsetToSessionID)
                    if (kvp.Key == connectingHeadset && kvp.Value == e.Sid)
                    {
                        HeadsetConnected(this, connectingHeadset);
                        connectingHeadset = null;
                    }
            }
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
                // automatically set up training handler with new session
                Cortex.training.sessionID = id;
                ctxClient.Subscribe(authorizer.CortexToken, id, Config.dataStreams);
                lock (locker)
                {
                    sessions[id] = new DataStream(id, Cortex.debugPrint);
                    headsetToSessionID[e.HeadsetId] = id;
                    dataSubscriber.AddStream(sessions[id], id, e.HeadsetId);
                }

                //HeadsetConnected(this, e.HeadsetId);
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
        /// Stops the connection to the Cortex service,
        /// called externally (preferably in OnApplicationQuit)
        /// </summary>
        public void Stop()
        {
            foreach (var k in sessions)
                ctxClient.UpdateSession(authorizer.CortexToken, k.Key, "close");
            sessions.Clear();
            headsetToSessionID.Clear();
        }

        /// <summary>
        /// Starts a cortex session with a given headset,
        /// triggers a callback that will also subscribe to data streams
        /// </summary>
        public void StartSession(string headsetID)
        {
            Debug.Log($"Attempting to start session with headset: {headsetID}");
            ctxClient.CreateSession(authorizer.CortexToken, headsetID, "open");
            connectingHeadset = headsetID;
        }
        /// <summary>
        /// Closes an individual session
        /// </summary>
        /// <param name="sessionID">the session to close</param>
        public void CloseSession(string sessionID)
        {
            ctxClient.UpdateSession(authorizer.CortexToken, sessionID, "close");
            sessions.Remove(sessionID);
            dataSubscriber.RemoveStreamBySessionID(sessionID);
            foreach (var item in headsetToSessionID.Where(kvp => kvp.Value == sessionID))
                headsetToSessionID.Remove(item.Key);
        }
    }
}
