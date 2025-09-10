namespace GameMain.Runtime
{
    public abstract class GameEventArgs : GameFramework.Event.GameEventArgs
    {
        private int? _id;

        public override int Id => _id ??= EventExtensions.GetEventId(GetType());
    }
}
