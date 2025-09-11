using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract class ShowSceneCommand : Command<Scene>
    {
        public ShowSceneCommand(Scene instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }
    }
}
