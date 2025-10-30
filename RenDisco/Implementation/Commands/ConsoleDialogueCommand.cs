using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Implementation.Commands
{
    internal class ConsoleDialogueCommand : DialogueCommand
    {
        public ConsoleDialogueCommand(Dialogue instruction) : base(instruction) {}

        public override IEnumerable<ControlFlowSignal> Flow()
        {
            
            Console.WriteLine(Instruction.Text);
            Thread.Sleep(3000);
            yield return null;
        }

        public override void Undo(){}
    }
}
