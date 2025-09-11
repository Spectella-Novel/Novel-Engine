using System;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public abstract class Command
    {
        protected Command(SynchronizationContext synchronizationContext)
        {
            SynchronizationContext = synchronizationContext;
        }

        protected SynchronizationContext SynchronizationContext { get; }

        public abstract InstructionResult Execute();
        public abstract void Undo();
        protected void InvokeInContext<T>(Action<T> action, T arg)
        {
            SynchronizationContext.Post(obj => { action.Invoke((T)obj); }, arg);
        }
        
    }
    public abstract class Command<T> : Command where T : Instruction 
    {
        protected T Instruction;
        protected Command(T instruction, SynchronizationContext synchronizationContext): base(synchronizationContext)
        {
            this.Instruction = instruction;
        }
    }
}
