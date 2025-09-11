using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    internal class ConsoleStopMusicCommand : StopMusicCommand
    {
        public ConsoleStopMusicCommand(StopMusic instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }

        public override InstructionResult Execute()
        {
            Console.WriteLine(Instruction.FadeOut == default ? $"Stop Music" : $"Stop Music: with fadeout of {Instruction.FadeOut} second(s)");
            return null;
        }

        public override void Undo()
        {
        }
    }
}
