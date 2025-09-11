using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class NarrationCommand : Command<Narration> 
    {
        public NarrationCommand(Narration instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }

        public override InstructionResult Execute()
        {
            return null;
        }

        public override void Undo()
        {

        }
    }
}
