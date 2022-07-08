using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EmotivUnityPlugin
{
    public class EventBufferInstance: MonoBehaviour
    {
        //public EventBufferBase buffer;
        List<EventBufferBase> buffers = new List<EventBufferBase>();

        private void Update()
        {
            foreach (EventBufferBase buffer in buffers)
                buffer.Process();
            //buffer.Process();
        }

        public void AddBuffer(EventBufferBase newBuffer) => buffers.Add(newBuffer);
    }

    public abstract class EventBufferBase
    {
        public abstract void Process();
    }

    public class EventBuffer<T> : EventBufferBase
    {
        private List<Action<T>> callbacks = new List<Action<T>>();

        T data;
        bool trigger;
        object locker = new object();

        public override void Process()
        {
            try {
                lock (locker)
                    if (trigger)
                    {
                        trigger = false;
                        foreach (Action<T> action in callbacks)
                            action(data);
                    }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Exception in event Buffer process: " + e);
            }
        }

        public void Subscribe(Action<T> action)
        {
            lock (locker)
                callbacks.Add(action);
        }

        public void Unsubscribe(Action<T> action)
        {
            lock (locker)
                callbacks.Remove(action);
        }

        public static EventBuffer<T> operator +(EventBuffer<T> lhs, Action<T> rhs)
        {
            lhs.Subscribe(rhs);
            return lhs;
        }
        public static EventBuffer<T> operator -(EventBuffer<T> lhs, Action<T> rhs)
        {
            lhs.Unsubscribe(rhs);
            return lhs;
        }

        public void OnParentEvent(object sender, T args)
        {
            try
            {
                lock (locker)
                {
                    trigger = true;
                    data = args;
                }
            }catch(Exception e)
            {
                Debug.LogWarning("Exception in event Buffer parent event: " + e);
            }
        }
    }
}
