using System;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class MenuCommand : Command<Menu>
    {
        public MenuCommand(Menu instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }

        public override InstructionResult Execute()
        {
            var result = new InstructionResult();
            Thread.Sleep(5000);
            SignalBroker.Emit("Input");
            var input = (int)WaitableMessageBroker.WaitForMessage("Input");
            SynchronizationContext.Post(a => Console.WriteLine(input), null);
            result.Instructions = Instruction.Choices[input].Response;
            
            return result;
        }

        public override void Undo()
        {
        }
    }
}
