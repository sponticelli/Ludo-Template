using System;
using System.Collections.Generic;

namespace Ludo.Events.Hub
{
    public sealed class EventHub : IEventHub
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> h) where T : IEvent
        {
            var k = typeof(T);
            if (!_subscribers.TryGetValue(k, out var l)) _subscribers[k] = l = new List<Delegate>();
            l.Add(h);
        }

        public void Unsubscribe<T>(Action<T> h) where T : IEvent
        {
            if (_subscribers.TryGetValue(typeof(T), out var l)) l.Remove(h);
        }

        public void Publish<T>(in T e) where T : IEvent
        {
            if (!_subscribers.TryGetValue(typeof(T), out var l)) return;
            for (int i = 0; i < l.Count; i++)
            {
                ((Action<T>)l[i])(e);
            }
        }
    }
}