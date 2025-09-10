using System;
using System.Collections.Generic;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static class EventExtensions
    {
        private static readonly Dictionary<Delegate, EventHandler<GameEventArgs>> Handlers = new Dictionary<Delegate, EventHandler<GameEventArgs>>();

        public static IUnsubscribe Subscribe<T>(this EventComponent eventComponent, EventHandler<T> handler)
            where T : GameEventArgs<T>
        {
            if (!Handlers.TryAdd(handler, Handler))
            {
                throw new ArgumentException($"Handler '{handler}' already exists.");
            }

            int eventId = GameEventArgs<T>.EventId;
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
            where T : GameEventArgs<T>
        {
            int eventId = GameEventArgs<T>.EventId;
            eventComponent.Unsubscribe(eventId, Handlers[handler]);
        }
    }
}
