using System;

namespace RenDisco
{
    public interface IStorage
    {
        public void Set(string key, object value);
        public object Get(string key);
    }
}
