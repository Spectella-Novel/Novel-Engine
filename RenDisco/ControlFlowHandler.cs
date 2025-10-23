using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco
{
    public interface IControlFlowHandler
    {
        public void Handle(ControlFlowSignal signal, Action action);
    }
    internal class ControlFlowHandler : IControlFlowHandler
    {


        public virtual void Handle(ControlFlowSignal signal, Action action)
        {
            return;
        }
    }
    internal class ContinueInInputThreadHandler : ControlFlowHandler
    {
        private readonly SynchronizationContext _syncContext;

        public ContinueInInputThreadHandler(SynchronizationContext syncContext)
        {
            _syncContext = syncContext;
        }

        public override void Handle(ControlFlowSignal signal, Action resumeExecution)
        {
            var waitHandle = new ManualResetEventSlim(false);
            _syncContext.Post(_ => {
                try
                {
                    resumeExecution();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                waitHandle.Set();
                }, 
                null);
            waitHandle.Wait(); // ждем завершения исполнения в главном потоке
        }
    }
}
