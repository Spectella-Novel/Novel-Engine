using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract class Command
    {
        public abstract IEnumerable<ControlFlowSignal> Flow();
        public abstract void Undo();
    }
    public abstract class Command<T> : Command where T : Instruction 
    {
        protected T Instruction;
        protected Command(T instruction): base()
        {
            this.Instruction = instruction;
        }
    }
}
