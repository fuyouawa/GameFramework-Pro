using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public class ContextComponent : GameFrameworkComponent
    {
        private readonly Dictionary<string, object> _contextByKey = new Dictionary<string, object>();

        public T Get<T>(string key, T defaultValue = default)
        {
            return (T)Get(key, (object)defaultValue);
        }

        public object Get(string key, object defaultValue = null)
        {
            if (_contextByKey.TryGetValue(key, out var value))
            {
                return value;
            }
            _contextByKey[key] = defaultValue;
            return defaultValue;
        }

        public bool Contains(string key)
        {
            return _contextByKey.ContainsKey(key);
        }

        public void Remove(string key)
        {
            _contextByKey.Remove(key);
        }

        public void Set(string key, object value)
        {
            _contextByKey[key] = value;
        }
    }
}
