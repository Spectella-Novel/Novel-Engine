using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    internal class ConsoleHideCommand : HideCommand
    {
        public ConsoleHideCommand(Hide instruction) : base(instruction)
        {
        }

        public override IEnumerable<ControlFlowSignal> Flow()
        {
            Console.WriteLine(Instruction.Transition == null ? $"Hide Image: {Instruction.Image}" : $"Hide Image: {Instruction.Image} with {Instruction.Transition} transition");
            return null;
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
