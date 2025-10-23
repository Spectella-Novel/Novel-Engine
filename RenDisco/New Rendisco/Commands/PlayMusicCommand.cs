using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract class PlayMusicCommand : Command<PlayMusic>
    {
        public PlayMusicCommand(PlayMusic instruction) : base(instruction)
        {
        }
    }
}
