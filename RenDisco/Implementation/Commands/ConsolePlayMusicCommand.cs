using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    public class ConsolePlayMusicCommand : PlayMusicCommand
    {
        public ConsolePlayMusicCommand(PlayMusic instruction) : base(instruction)
        {
        }

        public override IEnumerable<ControlFlowSignal> Flow()
        {
            Console.WriteLine(Instruction.FadeIn== default ? $"Play Music: {Instruction.File}" : $"Play Music: {Instruction.File} with fadein of {Instruction.FadeIn} second(s)");
            return null;
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
