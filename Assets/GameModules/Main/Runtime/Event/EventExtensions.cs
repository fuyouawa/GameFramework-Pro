using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static class EventExtensions
    {
        private static readonly Dictionary<Delegate, EventHandler<GameFramework.Event.GameEventArgs>> Handlers =
            new Dictionary<Delegate, EventHandler<GameFramework.Event.GameEventArgs>>();

        private static int s_nextEventId = 1000;
        private static readonly ConcurrentDictionary<Type, int> EventIdByType = new ConcurrentDictionary<Type, int>();

        private struct EventIdFastGetter<T>
        {
            public static readonly int EventId = GetEventId(typeof(T));
        }

        public static int GetEventId(Type eventType)
        {
            return EventIdByType.GetOrAdd(eventType, _ => Interlocked.Increment(ref s_nextEventId));
        }

        public static IUnsubscribe Subscribe<T>(this EventComponent eventComponent, EventHandler<T> handler)
            where T : GameEventArgs
        {
            if (!Handlers.TryAdd(handler, Handler))
            {
                throw new ArgumentException($"Handler '{handler}' already exists.");
            }

            int eventId = EventIdFastGetter<T>.EventId;
            eventComponent.Subscribe(eventId, Handler);
            return new UnsubscribeGeneric(() =>
            {
                Handlers.Remove(handler);
                eventComponent.Unsubscribe(eventId, Handler);
            });

            void Handler(object sender, GameFramework.Event.GameEventArgs e)
            {
                handler(sender, (T)e);
            }
        }

        public static void Unsubscribe<T>(this EventComponent eventComponent, EventHandler<T> handler)
            where T : GameEventArgs
        {
            int eventId = EventIdFastGetter<T>.EventId;
            eventComponent.Unsubscribe(eventId, Handlers[handler]);
        }
    }
}
