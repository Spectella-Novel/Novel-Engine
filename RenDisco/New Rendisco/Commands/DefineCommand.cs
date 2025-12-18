using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class DefineCommand : Command<Define>
    {
        protected IStorage Storage;
        public DefineCommand(Define instruction, IStorage storage) : base(instruction)
        {
            this.Storage = storage;
        }

        public override async UniTask<ControlFlowSignal> Execute()
        {
            return null;
        }

        public override void Undo()
        {
        }
    }
}
