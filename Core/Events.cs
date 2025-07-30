using System;
using System.Collections.Generic;

using Wc3_Combat_Game.Events;

namespace Wc3_Combat_Game.Core.Events
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent gameEvent) where TEvent : GameEvent;
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : GameEvent;
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : GameEvent;
    }

    public class EventBus: IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

        public void Publish<TEvent>(TEvent gameEvent) where TEvent : GameEvent
        {
            if(_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                // Iterate on a copy to prevent issues if handlers unsubscribe during iteration
                foreach(var handler in handlers.ToList())
                {
                    ((Action<TEvent>)handler).Invoke(gameEvent);
                }
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : GameEvent
        {
            if(!_handlers.ContainsKey(typeof(TEvent)))
            {
                _handlers[typeof(TEvent)] = new List<Delegate>();
            }
            _handlers[typeof(TEvent)].Add(handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : GameEvent
        {
            if(_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                handlers.Remove(handler);
                if(handlers.Count == 0)
                {
                    _handlers.Remove(typeof(TEvent));
                }
            }
        }
    }
}