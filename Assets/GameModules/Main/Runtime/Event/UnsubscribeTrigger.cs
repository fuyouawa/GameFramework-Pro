using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Runtime
{
    public abstract class UnsubscribeTrigger : MonoBehaviour
    {
        private readonly HashSet<IUnsubscribe> _unsubscribes = new HashSet<IUnsubscribe>();

        public void AddUnsubscribe(IUnsubscribe unsubscribe) => _unsubscribes.Add(unsubscribe);

        public void RemoveUnsubscribe(IUnsubscribe unsubscribe) => _unsubscribes.Remove(unsubscribe);

        public void Unsubscribe()
        {
            foreach (var unsubscribe in _unsubscribes)
            {
                unsubscribe.Unsubscribe();
            }

            _unsubscribes.Clear();
        }
    }

}
