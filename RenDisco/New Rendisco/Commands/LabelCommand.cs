using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class LabelCommand : Command<Label>
    {
        public LabelCommand(Label instruction) : base(instruction)
        {
        }

        public override IEnumerable<ControlFlowSignal> Flow()
        {
            var result = new ControlFlowSignal();
            result.Instructions = Instruction.Instructions;
            yield return result;
        }

        public override void Undo()
        {
        }
    }
}
