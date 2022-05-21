using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using System;
using System.Linq;

namespace EmotivUnityPlugin
{
    public class DataSubscriber : MonoBehaviour
    {
        public static DataSubscriber Instance;

        //List<DataStreamSubscriber> dataStreamSubscribers = new List<DataStreamSubscriber>();
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
            try
            {
                DataStreamEventBuffer dataStreamSubscriber = dataStreamSubscribers[headsetID];
                switch (typeof(T))
                {
                    case Type mType when mType == typeof(MentalCommand):
                        dataStreamSubscriber.MentalCommandReceived +=
                            (object sender, MentalCommand data) => callBack(data as T);
                        break;
                    case Type dType when dType == typeof(DevInfo):
                        dataStreamSubscriber.DevDataReceived +=
                            (object sender, DevInfo data) => callBack(data as T);
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
            if (!dataStreamSubscribers.ContainsKey(headsetID))
            {
                Debug.LogWarning("DataSubscriber: attempted to unsubscribe from a headset stream that doesn't exist");
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
                    case Type dType when dType == typeof(DevInfo):
                        dataStreamSubscriber.DevDataReceived -=
                            (object sender, DevInfo data) => callBack(data as T);
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

    public class DataStreamEventBuffer
    {
        DataStream dataStream;
        public string sessionID, headsetID;

        EventUpdater<MentalCommand> mentalCommandEventHandler = new EventUpdater<MentalCommand>();
        EventUpdater<DevInfo> devDataEventHandler = new EventUpdater<DevInfo>();
        EventUpdater<SysEventArgs> sysEventHandler = new EventUpdater<SysEventArgs>();

        public event EventHandler<MentalCommand> MentalCommandReceived
        {
            add { mentalCommandEventHandler.OnNewData += value; }
            remove { mentalCommandEventHandler.OnNewData -= value; }
        }
        public event EventHandler<DevInfo> DevDataReceived
        {
            add { devDataEventHandler.OnNewData += value; }
            remove { devDataEventHandler.OnNewData -= value; }
        }
        public event EventHandler<SysEventArgs> SysEventReceived
        {
            add { sysEventHandler.OnNewData += value; }
            remove { sysEventHandler.OnNewData -= value; }
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

        public class EventUpdater<T> where T : DataStreamEventArgs
        {
            public event EventHandler<T> OnNewData;

            bool newData;
            T data;

            public void Process()
            {
                if (newData)
                {
                    newData = false;
                    if (OnNewData != null)
                    {
                        //Debug.Log($"Sending out new data: {data}");
                        OnNewData(this, data);
                    }
                }
            }

            public void OnParentEvent(object sender, T args)
            {
                //Debug.Log($"New data received: {args}");
                data = args;
                newData = true;
            }
        }
    }
}
