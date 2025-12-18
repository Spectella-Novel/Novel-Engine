using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class IfConditionBlockCommand : Command<IfCondition>
    {
        public IfConditionBlockCommand(IfCondition instruction) : base(instruction)
        {
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
