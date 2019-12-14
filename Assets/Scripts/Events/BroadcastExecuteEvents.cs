using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

// https://weekly-geekly.github.io/articles/254239/index.html

namespace Proto2D
{
    public static class BroadcastReceivers
    {
        private static readonly IDictionary<Type, IList<GameObject>> BroadcstReceivers = new Dictionary<Type, IList<GameObject>>();

        public static IList<GameObject> GetHandlersForEvent<TEventType>() where TEventType : IEventSystemHandler
        {
            if (!BroadcstReceivers.ContainsKey(typeof(TEventType)))
            {
                return null;
            }
            return BroadcstReceivers[typeof(TEventType)];
        }

        public static void RegisterBroadcastReceiver<TEventType>(GameObject go) where TEventType : IEventSystemHandler
        {
            if (!BroadcstReceivers.ContainsKey(typeof(TEventType)))
            {
                BroadcstReceivers.Add(typeof(TEventType), new List<GameObject>());
            }

            BroadcstReceivers[typeof(TEventType)].Add(go);
        }

        public static void UnregisterBroadcastReceiver<TEventType>(GameObject go)
        {
            if (BroadcstReceivers.ContainsKey(typeof(TEventType)))
            {
                BroadcstReceivers[typeof(TEventType)].Remove(go);
            }
        }
    }

    public static class BroadcastExecuteEvents
    {
        public static void Execute<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
        {
            var handlers = BroadcastReceivers.GetHandlersForEvent<T>();

            if (handlers == null) return;

            foreach (var handler in handlers)
            {
                ExecuteEvents.Execute<T>(handler, eventData, functor);
            }
        }
    }
}
