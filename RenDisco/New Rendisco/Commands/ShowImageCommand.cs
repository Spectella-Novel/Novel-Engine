using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract class ShowImageCommand : Command<Show>
    {
        public ShowImageCommand(Show instruction) : base(instruction)
        {
        }
    }
}
