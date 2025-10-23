using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco
{
    public class AsyncStepper
    {
        private Task _gameTask;
        private SynchronizationContext _syncCtx;
        private GameFlow _game;
        private CancellationTokenSource _cancellationTokenSource;
        public AsyncStepper(List<Instruction> instructions, CommandFactory commandFactory)
        {
            _game = new GameFlow(instructions, commandFactory);
            _syncCtx = SynchronizationContext.Current;
            commandFactory.InitContext(_syncCtx);
        }
        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Stop() 
        { 
            _cancellationTokenSource.Cancel();
        }
    }
}
