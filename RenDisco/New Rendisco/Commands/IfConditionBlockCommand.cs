using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class IfConditionBlockCommand : Command<IfCondition>
    {
        public IfConditionBlockCommand(IfCondition instruction) : base(instruction)
        {
        }

        public override IEnumerable<ControlFlowSignal> Flow()
        {
            return null;
        }

        public override void Undo()
        {

        }
    }
}
