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
        public ConsoleDialogueCommand(Dialogue instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext) {}

        public override InstructionResult Execute()
        {
            
            Console.WriteLine(Instruction.Text);
            Thread.Sleep(3000);
            SynchronizationContext.Post(state => { Console.WriteLine(Thread.CurrentThread.Name + " Dialogue"); }, null);
            return null;
        }

        public override void Undo(){}
    }
}
