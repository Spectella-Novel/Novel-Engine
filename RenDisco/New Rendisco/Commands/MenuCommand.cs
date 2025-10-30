using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class MenuCommand : Command<Menu>
    {
        public MenuCommand(Menu instruction) : base(instruction)
        {
        }

        public override IEnumerable<ControlFlowSignal> Flow()
        {
            var result = new ControlFlowSignal();
            Thread.Sleep(5000);

            SignalBroker.Emit(DefaultSignals.Choice);
            
            var input = (int)WaitableMessageBroker.WaitForMessage(DefaultSignals.Choice);
            
            result.Instructions = Instruction.Choices[input].Response;
            
            yield return result;
        }

        public override void Undo()
        {
        }
    }
}
