using System;

namespace GameMain.Runtime
{
    public interface IUnsubscribe
    {
        void Unsubscribe();
    }

    public class UnsubscribeGeneric : IUnsubscribe
    {
        private readonly Action _onUnsubscribe;

        public UnsubscribeGeneric(Action onUnsubscribe)
        {
            _onUnsubscribe = onUnsubscribe;
        }

        public void Unsubscribe()
        {
            _onUnsubscribe?.Invoke();
        }
    }

}
