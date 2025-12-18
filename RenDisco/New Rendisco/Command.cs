using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract partial class Command
    {
        public abstract UniTask<ControlFlowSignal> Execute();
        public abstract void Undo();
    }
    public abstract partial class Command<T> : Command where T : Instruction 
    {
        protected T Instruction;
        protected Command(T instruction): base()
        {
            this.Instruction = instruction;
        }
    }
}
