using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EmotivUnityPlugin
{
    public class EventBufferInstance: MonoBehaviour
    {
        public EventBufferBase buffer;

        private void Update()
        {
            buffer.Process();
        }
    }

    public abstract class EventBufferBase
    {
        public abstract void Process();
    }

    public class EventBuffer<T> : EventBufferBase
    {
        public event EventHandler<T> Event;

        T data;
        bool trigger;

        public override void Process()
        {
            if (trigger)
            {
                trigger = false;
                if (Event != null)
                    Event(this, data);
            }
        }

        public void Subscribe(Action<T> action)
        {
            Event += (object o, T args) => action(args);
        }

        public void Unsubscribe(Action<T> action)
        {
            Event -= (object o, T args) => action(args);
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
            trigger = true;
            data = args;
        }
    }
}
