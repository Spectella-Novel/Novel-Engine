using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace RenDisco.Commands
{
    public class JumpCommand : Command<Jump>
    {
        public JumpCommand(Jump instruction) : base(instruction)
        {
        }

        public override async UniTask<ControlFlowSignal> Execute()
        {
            return ControlFlowSignal.Jump(Instruction.Label);
        }

        public override void Undo()
        {
       
        }
    }
}
