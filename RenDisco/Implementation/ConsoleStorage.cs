using System.Collections.Generic;

namespace RenDisco.Implementation
{
    public class ConsoleStorage : IStorage
    {
        Dictionary<string, object> _storage;
        public ConsoleStorage()
        {
            _storage = new Dictionary<string, object>();
        }
        public object Get(string key)
        {
            _storage.TryGetValue(key, out object value);
            return value;
        }

        public void Set(string key, object value)
        {
            _storage[key] = value;
        }
    }

}
