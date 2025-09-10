using System;
using System.Collections.Generic;
using System.Threading;
using GameFramework.Event;
using JetBrains.Annotations;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static class EventExtensions
    {
        private static readonly Dictionary<Delegate, EventHandler<GameEventArgs>> Handlers = new Dictionary<Delegate, EventHandler<GameEventArgs>>();

        private static int s_nextEventId = 1000;
        private struct EventIdGetter<[UsedImplicitly] T>
        {
            public static readonly int EventId = Interlocked.Increment(ref s_nextEventId);
        }

        public static int GetEventId<T>()
            where T : GameEventArgs
        {
            return EventIdGetter<T>.EventId;
        }

        public static IUnsubscribe Subscribe<T>(this EventComponent eventComponent, EventHandler<T> handler)
            where T : GameEventArgs
        {
            if (!Handlers.TryAdd(handler, Handler))
            {
                throw new ArgumentException($"Handler '{handler}' already exists.");
            }

            int eventId = GetEventId<T>();
            eventComponent.Subscribe(eventId, Handler);
            return new UnsubscribeGeneric(() =>
            {
                Handlers.Remove(handler);
                eventComponent.Unsubscribe(eventId, Handler);
            });

            void Handler(object sender, GameEventArgs e)
            {
                handler(sender, (T)e);
            }
        }

        public static void Unsubscribe<T>(this EventComponent eventComponent, EventHandler<T> handler)
            where T : GameEventArgs
        {
            int eventId = GetEventId<T>();
            eventComponent.Unsubscribe(eventId, Handlers[handler]);
        }
    }
}
