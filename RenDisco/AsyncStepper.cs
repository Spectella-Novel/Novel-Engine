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
        private Game _game;
        private CancellationTokenSource _cancellationTokenSource;
        public AsyncStepper(List<Instruction> instructions, CommandFactory commandFactory)
        {
            _game = new Game(instructions, commandFactory);
            _syncCtx = SynchronizationContext.Current;
            commandFactory.InitContext(_syncCtx);
        }
        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _gameTask = Task.Run(() => { GameStep(); }, _cancellationTokenSource.Token);
        }
        private async void GameStep()
        {

            while (_game.IsCompleted())
            {
                _game.Step();
            }

        }
        public void Stop() 
        { 
            _cancellationTokenSource.Cancel();
        }
    }
}
