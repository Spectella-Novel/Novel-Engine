using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class ElIfConditionalBlockCommand : Command<ElifCondition>
    {
        public ElIfConditionalBlockCommand(ElifCondition instruction) : base(instruction)
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
