using System;

namespace GameMain.Runtime
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HotUpdateEntranceAttribute : Attribute
    {
    }
}
