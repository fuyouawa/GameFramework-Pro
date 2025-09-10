using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Runtime
{
    public abstract class UnregisterTrigger : MonoBehaviour
    {
        private readonly HashSet<IUnsubscribe> _unregisters = new HashSet<IUnsubscribe>();

        public void AddUnregister(IUnsubscribe unsubscribe) => _unregisters.Add(unsubscribe);

        public void RemoveUnregister(IUnsubscribe unsubscribe) => _unregisters.Remove(unsubscribe);

        public void Unregister()
        {
            foreach (var unregister in _unregisters)
            {
                unregister.Unsubscribe();
            }

            _unregisters.Clear();
        }
    }

    public class UnregisterOnDestroyTrigger : UnregisterTrigger
    {
        private void OnDestroy()
        {
            Unregister();
        }
    }

    public class UnregisterOnDisableTrigger : UnregisterTrigger
    {
        private void OnDestroy()
        {
            Unregister();
        }

        private void OnDisable()
        {
            Unregister();
        }
    }

    public static class UnsubscribeExtensions
    {
        public static IUnsubscribe UnregisterWhenDestroyed(
            this IUnsubscribe unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnregisterOnDestroyTrigger>();
            trigger.AddUnregister(unRegister);
            return unRegister;
        }

        public static IUnsubscribe UnregisterWhenDisabled(
            this IUnsubscribe unRegister,
            GameObject gameObject)
        {
            var trigger = gameObject.GetOrAddComponent<UnregisterOnDisableTrigger>();
            trigger.AddUnregister(unRegister);
            return unRegister;
        }
    }
}
