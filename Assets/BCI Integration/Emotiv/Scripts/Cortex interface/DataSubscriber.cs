using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System;
using System.Linq;

namespace EmotivUnityPlugin
{
    /// <summary>
    /// Provides buffered events from active data streams to Unity synchronous events
    /// </summary>
    public class DataSubscriber : MonoBehaviour
    {
        public static DataSubscriber Instance;

        Dictionary<string, DataStreamEventBuffer> dataStreamSubscribers = new Dictionary<string, DataStreamEventBuffer>();

        DataSubscriber()
        {
            if (!Instance)
                Instance = this;
        }

        void Update()
        {
            foreach (var subscriber in dataStreamSubscribers.Values)
                subscriber.Process();
        }

        /// <summary>
        /// Add a new data stream to the event handling process
        /// </summary>
        /// <param name="newStream">Stream to add</param>
        /// <param name="sessionID">id of the corresponding session</param>
        /// <param name="headsetID">id of the relevant headset</param>
        public void AddStream(DataStream newStream, string sessionID, string headsetID)
        {
            try
            {
                dataStreamSubscribers[headsetID] = new DataStreamEventBuffer(newStream, sessionID, headsetID);
                print("New stream added");
            }catch(Exception e)
            {
                print(e);
            }
        }
        public void RemoveStreamByHeadsetID(string id)
        {
            dataStreamSubscribers.Remove(id);
        }
        public void RemoveStreamBySessionID(string id)
        {
            foreach (var item in dataStreamSubscribers.Where(kvp => kvp.Value.sessionID == id))
            {
                dataStreamSubscribers.Remove(item.Key);
            }
        }

        /// <summary>
        /// Connects the provided callback function to the typed
        /// data stream of the given headset, provided it exists.
        /// This callback will be wrapped in Unity's thread and Update callback,
        /// making it able to trigger updates to game state
        /// </summary>
        /// <typeparam name="T">The type of data to subscribe to</typeparam>
        /// <param name="headsetID">ID of the desired headset stream</param>
        /// <param name="callBack">Function to be called</param>
        /// <returns>true if successful</returns>
        public bool SubscribeDataStream<T>(string headsetID, Action<T> callBack) where T : DataStreamEventArgs
        {
            if (string.IsNullOrEmpty(headsetID) || !dataStreamSubscribers.ContainsKey(headsetID))
            {
                Debug.LogWarning("DataSubscriber: attempted to Subscribe to a headset stream that doesn't exist");
                return false;
            }
            try
            {
                DataStreamEventBuffer dataStreamSubscriber = dataStreamSubscribers[headsetID];
                switch (typeof(T))
                {
                    case Type mType when mType == typeof(MentalCommand):
                        dataStreamSubscriber.MentalCommandReceived +=
                            (object sender, MentalCommand data) => callBack(data as T);
                        break;
                    case Type dType when dType == typeof(DeviceInfo):
                        dataStreamSubscriber.DevDataReceived +=
                            (object sender, DeviceInfo data) => callBack(data as T);
                        break;
                    case Type sType when sType == typeof(SysEventArgs):
                        dataStreamSubscriber.SysEventReceived +=
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
        /// DiscConnects the provided callback function from the typed
        /// data stream of the given headset, provided it exists.
        /// </summary>
        /// <typeparam name="T">The type of data to subscribe to</typeparam>
        /// <param name="headsetID">ID of the desired headset stream</param>
        /// <param name="callBack">Function to be called</param>
        /// <returns>true if successful</returns>
        public bool UnsubscribeDataStream<T>(string headsetID, Action<T> callBack) where T : DataStreamEventArgs
        {
            if (string.IsNullOrEmpty(headsetID) || !dataStreamSubscribers.ContainsKey(headsetID))
            {
                Debug.LogWarning("DataSubscriber: attempted to Unsubscribe from a headset stream that doesn't exist");
                return false;
            }

            try
            {
                DataStreamEventBuffer dataStreamSubscriber = dataStreamSubscribers[headsetID];
                switch (typeof(T))
                {
                    case Type mType when mType == typeof(MentalCommand):
                        dataStreamSubscriber.MentalCommandReceived -=
                            (object sender, MentalCommand data) => callBack(data as T);
                        break;
                    case Type dType when dType == typeof(DeviceInfo):
                        dataStreamSubscriber.DevDataReceived -=
                            (object sender, DeviceInfo data) => callBack(data as T);
                        break;
                    case Type sType when sType == typeof(SysEventArgs):
                        dataStreamSubscriber.SysEventReceived -=
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

    }

    /// <summary>
    /// Specialized Event Buffer than handles desired streams of each open session
    /// You will need to add to this to manage additional data streams
    /// </summary>
    public class DataStreamEventBuffer
    {
        DataStream dataStream;
        public string sessionID, headsetID;

        EventBuffer<MentalCommand> mentalCommandEventHandler = new EventBuffer<MentalCommand>();
        EventBuffer<DeviceInfo> devDataEventHandler = new EventBuffer<DeviceInfo>();
        EventBuffer<SysEventArgs> sysEventHandler = new EventBuffer<SysEventArgs>();

        public event EventHandler<MentalCommand> MentalCommandReceived
        {
            add { mentalCommandEventHandler.Event += value; }
            remove { mentalCommandEventHandler.Event -= value; }
        }
        public event EventHandler<DeviceInfo> DevDataReceived
        {
            add { devDataEventHandler.Event += value; }
            remove { devDataEventHandler.Event -= value; }
        }
        public event EventHandler<SysEventArgs> SysEventReceived
        {
            add { sysEventHandler.Event += value; }
            remove { sysEventHandler.Event -= value; }
        }

        public DataStreamEventBuffer(DataStream stream, string sessionID, string headsetID)
        {
            dataStream = stream;
            this.sessionID = sessionID;
            this.headsetID = headsetID;
            dataStream.MentalCommandReceived += mentalCommandEventHandler.OnParentEvent;
            dataStream.DevDataReceived += devDataEventHandler.OnParentEvent;
            dataStream.SysEventReceived += sysEventHandler.OnParentEvent;
        }

        public void Process()
        {
            mentalCommandEventHandler.Process();
            devDataEventHandler.Process();
            sysEventHandler.Process();
        }
    }
}