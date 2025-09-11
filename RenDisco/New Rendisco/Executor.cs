using RenDisco.Commands;
using System;
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
        public InstructionResult ExecuteCommand(Instruction instruction)
        {
            var command = _factory.CreateCommand(instruction);
            var result = command.Execute();

            return result;

        }
    }
}
