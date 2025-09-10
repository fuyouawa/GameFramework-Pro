namespace GameMain.Runtime
{
    public class UnsubscribeOnDisableTrigger : UnsubscribeTrigger
    {
        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }
    }
}
