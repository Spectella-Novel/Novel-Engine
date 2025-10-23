using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco
{
    internal class ThreadController
    {
        Dictionary<string, SynchronizationContext> _synchronizationContexts = new();
        public void ExecuteIn(string threadName, Action action)
        {

        }
        public void RememberThread(string threadName)
        {
            if(_synchronizationContexts.ContainsKey()
            _synchronizationContexts.Add(threadName, new SynchronizationContext());
        }
    }
}
