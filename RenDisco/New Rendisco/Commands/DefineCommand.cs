using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract class DefineCommand : Command<Define>
    {
        protected IStorage Storage;
        public DefineCommand(Define instruction, IStorage storage, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
            this.Storage = storage;
        }
    }
}
