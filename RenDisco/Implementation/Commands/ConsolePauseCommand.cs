using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    public class ConsolePauseCommand : PauseCommand
    {
        public ConsolePauseCommand(Pause instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }

        public override InstructionResult Execute()
        {

            Console.WriteLine(Instruction.Duration == default ? $"Pause" : $"Pause: {Instruction.Duration} second(s)");
            return null;
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
