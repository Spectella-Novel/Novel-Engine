using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace RenDisco
{
    internal class Executor
    {
        private readonly CommandFactory _factory;

        public Executor(CommandFactory commandFactory)
        {
            _factory = commandFactory;
        }
        public IEnumerable<ControlFlowSignal> GetFlow(Instruction instruction)
        {
            var command = _factory.CreateCommand(instruction);
            var flow = command.Flow();
            return flow;
        }
    }
}
