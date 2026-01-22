using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class DebugCommand : Command<DebugLog>
    {
        public DebugCommand(DebugLog instruction, IStorage storage) : base(instruction)
        {
            Storage = storage;
        }

        public IStorage Storage { get; }

        public override async UniTask<ControlFlowSignal> Execute()
        {
            string expression =  Instruction.Expression.Evaluate(Storage).ToString();
            Console.WriteLine($"[Debug] {expression}");
            return null;
        }

        public override void Undo()
        {
            
        }
    }
}
