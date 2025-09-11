using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract class HideCommand : Command<Hide>
    {
        public HideCommand(Hide instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }
    }
}
