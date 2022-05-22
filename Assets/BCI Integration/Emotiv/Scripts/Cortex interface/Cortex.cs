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
        public static bool debugPrint;

        // Event buffers, enable event calls within Unity from other threads
        static EventBuffer<List<Headset>> queryHeadsetBuffer;
        static EventBuffer<string> headsetConnectedBuffer;
        
        // Events called from event buffers
        public static event EventHandler<List<Headset>> QueryHeadsetOK
        {
            add { queryHeadsetBuffer.Event += value; }
            remove { queryHeadsetBuffer.Event -= value; }
        }
        public static event EventHandler<string> HeadsetConnected
        {
            add { headsetConnectedBuffer.Event += value; }
            remove { headsetConnectedBuffer.Event -= value; }
        }

        /*
         * Initiate
         * Stop
         * 
         * start session
         * close session
         * query headsets
         * 
         * 
         * sub/unsub to:
         *      data streams, including mental commands, devInfo, and system events by headset session
         *      headset list
         *      profile list
         *      
         * training:
         *      query profile list
         *      create profile
         *      load profile with headset
         *      unload profile
         *      get current profile
         *      save profile
         *      satrt training
         *      accept training
         *      reject training
         *      erase training
         *      reset training
         *      
         *      sub/unsub to:
         *          query profile ok
         *          create profile ok
         *          profile saved ok
         *          training ok
         *          get detection info ok
         *          profile loaded
         *          profile unloaded
         *          get current profile done
         * */

        /// <summary>
        /// Initiates the authorizer and cortex client,
        /// called externally at the beginning of the program
        /// </summary>
        /// <param name="debug">enable verbose logs</param>
        /// <param name="license">uneccessary in most cases,
        /// if you need this you probably know what you are doing and will be changing this code anyways</param>
        public static void Start(bool debug = false, string license = "")
        {
            debugPrint = debug;

            // create Event Buffer GameObject to drive in engine events
            GameObject eventBufferObject = new GameObject();
            GameObject.DontDestroyOnLoad(eventBufferObject);
            eventBufferObject.name = "Event Buffer Object";

            // add data subscriber (data stream event buffer handler)
            dataSubscriber = eventBufferObject.AddComponent<DataSubscriber>();

            // add buffer for headset query completion
            queryHeadsetBuffer = new EventBuffer<List<Headset>>();
            headsetFinder.QueryHeadsetOK += queryHeadsetBuffer.OnParentEvent;
            eventBufferObject.AddComponent<EventBufferInstance>().buffer = queryHeadsetBuffer;

            //add buffer for headset connection
            headsetConnectedBuffer = new EventBuffer<string>();
            dataStreamManager.HeadsetConnected += headsetConnectedBuffer.OnParentEvent;
            eventBufferObject.AddComponent<EventBufferInstance>().buffer = headsetConnectedBuffer;

            // Initiate data stream manager
            dataStreamManager.Init();
            // Initialize websocket client
            ctxClient.InitWebSocketClient();
            // Start connecting to cortex service
            authorizer.StartAction(license);
        }
        public static void Stop()
        {
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
        /// Trigger a query into the availaale headsets,
        /// subscribe to QueryHeadsetOK for result
        /// </summary>
        public static void QueryHeadsets()
        {
            headsetFinder.TriggerQuery();
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
        public static bool SubscribeSysEvents(string headsetID, Action<DeviceInfo> action)
            => SubscribeDataStream(headsetID, action);
        /// <summary>
        /// Simplified interface to UnsubscribeDataStream for system events
        /// </summary>
        public static bool UnsubscribeSysEvents(string headsetID, Action<DeviceInfo> action)
            => UnsubscribeDataStream(headsetID, action);
    }
}
