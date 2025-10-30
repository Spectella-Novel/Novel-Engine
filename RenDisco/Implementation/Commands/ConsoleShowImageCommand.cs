using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    internal class ConsoleShowImageCommand : ShowImageCommand
    {
        public ConsoleShowImageCommand(Show instruction) : base(instruction) { }

        public override IEnumerable<ControlFlowSignal> Flow()
        {
            return null;
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
