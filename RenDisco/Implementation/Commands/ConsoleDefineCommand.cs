using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    internal class ConsoleDefineCommand : DefineCommand
    {
        public ConsoleDefineCommand(Define instruction, IStorage storage, SynchronizationContext synchronizationContext) : base(instruction, storage, synchronizationContext)
        {
        }

        public override ControlFlowSignal Flow() { 

            Storage.Set(Instruction.Name, Instruction.Value);
            return null;

        }

        public override void Undo()
        {
            
        }
    }
}
