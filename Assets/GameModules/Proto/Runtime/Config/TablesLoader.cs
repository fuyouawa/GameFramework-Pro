using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace GameProto.Runtime
{
    public static class TablesLoader
    {
        public static UniTask LoadTablesAsync(Func<Type, string, UniTask> loadTableAsync)
        {
            var tasks = new List<UniTask>
            {
                loadTableAsync(typeof(Test), "test_tbtest")
            };
            return UniTask.WhenAll(tasks);
        }
    }
}
