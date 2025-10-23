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

        public override ControlFlowSignal Flow()
        {
            var result = new ControlFlowSignal();
            result.Instructions = Instruction.Instructions;
            return result;
        }

        public override void Undo()
        {
        }
    }
}
