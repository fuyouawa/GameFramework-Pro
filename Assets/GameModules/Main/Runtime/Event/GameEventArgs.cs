namespace GameMain.Runtime
{
    public abstract class GameEventArgs<T> : GameFramework.Event.GameEventArgs
        where T : GameEventArgs<T>
    {
        private static int s_nextEventId = 0;
        public static readonly int EventId = s_nextEventId++;

        public override int Id => s_nextEventId;
    }
}
