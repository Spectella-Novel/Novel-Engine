using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract class DialogueCommand : Command<Dialogue>
    {
        public DialogueCommand(Dialogue instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }
    }
}
