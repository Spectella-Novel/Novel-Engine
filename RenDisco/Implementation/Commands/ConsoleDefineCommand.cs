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
        public ConsoleDefineCommand(Define instruction, IStorage storage) : base(instruction, storage)
        {
        }

        public override IEnumerable<ControlFlowSignal> Flow() { 

            Storage.Set(Instruction.Name, Instruction.Value);
            return null;
        }

        public override void Undo()
        {
            
        }
    }
}
