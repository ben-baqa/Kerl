using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        static DataSubscriber dataSubscriber;

        // verbose logs
        public static bool debugPrint;

        /*
         * Initiate
         * Stop
         * 
         * start session
         * close session
         * 
         * sub/unsub to:
         *      data streams, including mental commands, devInfo, and system events by headset session
         *      headset list
         *      profile list
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

            // create DataSubscriber gameobject to drive in engine events
            GameObject dataSubscriberGameObject = new GameObject();
            dataSubscriberGameObject.name = "DataSubscriber Object";
            dataSubscriber = dataSubscriberGameObject.AddComponent<DataSubscriber>();

            // Initiate data stream manager
            dataStreamManager.Init();
            // Initialize websocket client
            ctxClient.InitWebSocketClient();
            // Start connecting to cortex service
            authorizer.StartAction(license);
        }
        public static void Stop()
        {

        }
    }
}
