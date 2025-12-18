using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class LabelCommand : Command<Label>
    {
        public LabelCommand(Label instruction) : base(instruction)
        {
        }

        public override async UniTask<ControlFlowSignal> Execute()
        {
            return ControlFlowSignal.Down(Instruction.Instructions);
        }

        public override void Undo()
        {
        }
    }
}
