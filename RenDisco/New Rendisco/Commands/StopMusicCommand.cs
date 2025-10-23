using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract class StopMusicCommand : Command<StopMusic>
    {
        public StopMusicCommand(StopMusic instruction) : base(instruction)
        {
        }
    }
}
