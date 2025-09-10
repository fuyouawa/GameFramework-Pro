namespace GameMain.Runtime
{
    public class UnsubscribeOnDestroyTrigger : UnsubscribeTrigger
    {
        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}
