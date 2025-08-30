using System;

namespace Ludo.Events.Hub
{
    /// <summary>
    /// A Global Event Hub
    /// </summary>
    public interface IEventHub 
    {
        void Subscribe<T>(Action<T> h) where T : IEvent;
        void Unsubscribe<T>(Action<T> h) where T : IEvent;
        void Publish<T>(in T e) where T : IEvent;
    }
}