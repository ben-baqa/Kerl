using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EmotivUnityPlugin
{
    /// <summary>
    /// Primary interface for Emotiv integration
    /// </summary>
    public static class Cortex
    {
        static CortexClient ctxClient = CortexClient.Instance;
        static Authorizer authorizer = Authorizer.Instance;
        static DataStreamManager dataStreamManager = DataStreamManager.Instance;
        static HeadsetFinder headsetFinder = HeadsetFinder.Instance;
        static DataSubscriber dataSubscriber;

        // verbose logs
        public static bool printLogs;
        public static bool printStreamData;
        public static bool isQuitting;

        // Event buffers, enable event calls within Unity from other threads
        public static EventBuffer<List<Headset>> HeadsetQueryResult;
        public static EventBuffer<string> DataStreamStarted;
        public static EventBuffer<HeadsetConnectEventArgs> HeadsetConnected;
        public static EventBuffer<string> HeadsetDisconnected;
        public static EventBuffer<ConnectToCortexStates> ConnectionStateChanged;
        public static EventBuffer<ErrorMsgEventArgs> ErrorRecieved;

        public static TrainingHandler training;
        public static TrainingHandler profiles => training;

        /// <summary>
        /// Initiates the authorizer and cortex client,
        /// called externally at the beginning of the program.
        /// It is best to call this in Awake() from a script with execution priority
        /// </summary>
        /// <param name="logs">enable verbose logs</param>
        /// <param name="streamPrint">enable continous prints of incoming stream data</param>
        /// <param name="license">uneccessary in most cases,
        /// if you need this you probably know what you are doing and will be changing this code anyways</param>
        public static void Start(bool logs = false, bool streamPrint = false, string license = "")
        {
            printLogs = logs;
            printStreamData = streamPrint;

            // create Event Buffer GameObject to drive in engine events
            GameObject eventBufferObject = new GameObject();
            GameObject.DontDestroyOnLoad(eventBufferObject);
            eventBufferObject.name = "Event Buffer Object";
            EventBufferInstance eventBufferInstance = eventBufferObject.AddComponent<EventBufferInstance>();

            // add data subscriber (data stream event buffer handler)
            dataSubscriber = eventBufferObject.AddComponent<DataSubscriber>();

            // add buffer for headset query completion
            HeadsetQueryResult = new EventBuffer<List<Headset>>();
            headsetFinder.QueryHeadsetOK += HeadsetQueryResult.OnParentEvent;

            // add buffer for successful start of data stream (initating session with headset)
            DataStreamStarted = new EventBuffer<string>();
            dataStreamManager.DataStreamStarted += DataStreamStarted.OnParentEvent;

            // add buffer for headset connection (pairing with computer)
            HeadsetConnected = new EventBuffer<HeadsetConnectEventArgs>();
            ctxClient.HeadsetConnectNotify += HeadsetConnected.OnParentEvent;

            // add buffer for when a headset is unexpectedly disconnected (sends session ID)
            HeadsetDisconnected = new EventBuffer<string>();
            dataStreamManager.HeadsetDisconnected += HeadsetDisconnected.OnParentEvent;

            // add buffer for connection state changing
            ConnectionStateChanged = new EventBuffer<ConnectToCortexStates>();
            authorizer.ConnectServiceStateChanged += ConnectionStateChanged.OnParentEvent;

            // add buffer for error recieved
            ErrorRecieved = new EventBuffer<ErrorMsgEventArgs>();
            ctxClient.ErrorMsgReceived += ErrorRecieved.OnParentEvent;

            EventBufferBase[] buffers = new EventBufferBase[]
            {
                HeadsetQueryResult,
                DataStreamStarted,
                HeadsetConnected,
                HeadsetDisconnected,
                ConnectionStateChanged,
                ErrorRecieved
            };
            eventBufferInstance.AddBuffers(buffers);

            // initialize Training handler
            training = new TrainingHandler();
            training.InstantiateEventBuffers(eventBufferInstance);

            // Initiate data stream manager
            dataStreamManager.Init();
            // Initialize websocket client
            ctxClient.InitWebSocketClient();
            // Start connecting to cortex service
            authorizer.StartAction(license);
        }
        public static void Stop()
        {
            isQuitting = true;
            dataStreamManager.Stop();
            HeadsetFinder.Instance.StopQueryHeadset();
            ctxClient.ForceCloseWSC();
        }

        /// <summary>
        /// Start a session with the given headset,
        /// will automatically subscribe to basic data streams
        /// and trigger HeadsetConnected event
        /// </summary>
        /// <param name="headsetID"></param>
        public static void StartSession(string headsetID) => dataStreamManager.StartSession(headsetID);
        /// <summary>
        /// Ends the sessions specified by the given ID
        /// </summary>
        public static void EndSession(string sessionID) => dataStreamManager.CloseSession(sessionID);
        /// <summary>
        /// Edns the session specified by the given headsetID
        /// </summary>
        public static void EndSessionByHeadset(string headsetID) => dataStreamManager.CloseSessionByHeadset(headsetID);
        /// <summary>
        /// Ends the session created most recently
        /// </summary>
        public static void EndMostRecentSession() => dataStreamManager.CloseMostRecentSession();

        /// <summary>
        /// Trigger a query into the availaale headsets,
        /// subscribe to QueryHeadsetOK for result
        /// </summary>
        public static void QueryHeadsets()
        {
            headsetFinder.TriggerQuery();
        }

        /// <summary>
        /// Checks if a given headset is already in use
        /// </summary>
        /// <param name="headsetID">headset to check</param>
        /// <returns>true if there is already an extant session for the headset</returns>
        public static bool HeadsetIsAlreadyInUse(string headsetID)
        {
            return dataStreamManager.HeadsetIsAlreadyInUse(headsetID);
        }

        /// <summary>
        /// Connect a Device that is discovered, but unavailable
        /// (bluetooth pairing, basically)
        /// </summary>
        public static void ConnectDevice(string headsetID)
        {
            ctxClient.ConnectDevice(headsetID);
        }

        /// <summary>
        /// Subscribe to incoming data stream events for the given headset,
        /// the provided function will be called when new data is received.
        /// The headset must first be paired, and have an active session
        /// </summary>
        /// <typeparam name="T">type of data to recieve</typeparam>
        /// <param name="headsetID">headset to get data from</param>
        /// <param name="action">method to be called when new data is recieved</param>
        /// <returns>true if subscription was successful</returns>
        public static bool SubscribeDataStream<T>(string headsetID, Action<T> action) where T: DataStreamEventArgs
            => dataSubscriber.SubscribeDataStream(headsetID, action);
        /// <summary>
        /// Unsubscribe from incoming data stream events for the given headset,
        /// all subscriptions will be cleared automatically on session closure,
        /// but it is efficient to unsubscribe when the data feed is uneccesary
        /// </summary>
        /// <typeparam name="T">type of data subscription</typeparam>
        /// <param name="headsetID">headset of the desired stream</param>
        /// <param name="action">method to remove from callback</param>
        /// <returns>true if unsubscription was successful</returns>
        public static bool UnsubscribeDataStream<T>(string headsetID, Action<T> action) where T : DataStreamEventArgs
            => dataSubscriber.UnsubscribeDataStream(headsetID, action);

        /// <summary>
        /// Simplified interface to SubscribeDataStream for mental commands
        /// </summary>
        public static bool SubscribeMentalCommands(string headsetID, Action<MentalCommand> action)
            => SubscribeDataStream(headsetID, action);
        /// <summary>
        /// Simplified interface to UnsubscribeDataStream for mental commands
        /// </summary>
        public static bool UnsubscribeMentalCommands(string headsetID, Action<MentalCommand> action)
            => UnsubscribeDataStream(headsetID, action);

        /// <summary>
        /// Simplified interface to SubscribeDataStream for device information
        /// </summary>
        public static bool SubscribeDeviceInfo(string headsetID, Action<DeviceInfo> action)
            => SubscribeDataStream(headsetID, action);
        /// <summary>
        /// Simplified interface to UnsubscribeDataStream for devide information
        /// </summary>
        public static bool UnsubscribeDeviceInfo(string headsetID, Action<DeviceInfo> action)
            => UnsubscribeDataStream(headsetID, action);

        /// <summary>
        /// Simplified interface to SubscribeDataStream for system events
        /// </summary>
        public static bool SubscribeSysEvents(string headsetID, Action<SysEventArgs> action)
            => SubscribeDataStream(headsetID, action);
        /// <summary>
        /// Simplified interface to UnsubscribeDataStream for system events
        /// </summary>
        public static bool UnsubscribeSysEvents(string headsetID, Action<SysEventArgs> action)
            => UnsubscribeDataStream(headsetID, action);
    }
}
