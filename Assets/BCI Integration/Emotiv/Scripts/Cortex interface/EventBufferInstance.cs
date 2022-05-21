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

        public void OnParentEvent(object sender, T args)
        {
            trigger = true;
            data = args;
        }
    }
}
