using RenDisco.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    internal class ConsoleShowImageCommand : ShowImageCommand
    {
        public ConsoleShowImageCommand(Show instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext) { }

        public override ControlFlowSignal Flow()
        {
            return null;
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
