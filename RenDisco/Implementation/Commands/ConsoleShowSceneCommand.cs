using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    internal class ConsoleShowSceneCommand : ShowSceneCommand
    {
        public ConsoleShowSceneCommand(Scene instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }

        public override ControlFlowSignal Flow()
        {
            Console.WriteLine(Instruction.Transition == null ? $"Show Image: {Instruction.Image}" : $"Show Image: {Instruction.Image} with {Instruction.Transition} transition");
            return null;
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
