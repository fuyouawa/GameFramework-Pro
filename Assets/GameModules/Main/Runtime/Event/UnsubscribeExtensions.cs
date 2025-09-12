using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static class UnsubscribeExtensions
    {
        public static IUnsubscribe UnsubscribeWhenDestroyed(
            this IUnsubscribe unsubscribe,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnsubscribeOnDestroyTrigger>();
            trigger.AddUnsubscribe(unsubscribe);
            return unsubscribe;
        }

        public static IUnsubscribe UnsubscribeWhenDisabled(
            this IUnsubscribe unsubscribe,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnsubscribeOnDisableTrigger>();
            trigger.AddUnsubscribe(unsubscribe);
            return unsubscribe;
        }
    }
}
