using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    internal class ConsoleNarrationCommand : NarrationCommand
    {
        public ConsoleNarrationCommand(Narration instruction) : base(instruction)
        {
        }

        public override IEnumerable<ControlFlowSignal> Flow()
        {
            Console.WriteLine($"Narration: {Instruction.Text}");
            return null;
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
