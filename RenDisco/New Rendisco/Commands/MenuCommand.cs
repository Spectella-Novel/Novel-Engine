using Cysharp.Threading.Tasks;
using System.Threading;

namespace RenDisco.Commands
{
    public class MenuCommand : Command<Menu>
    {
        public MenuCommand(Menu instruction) : base(instruction)
        {
        }

        public override async UniTask<ControlFlowSignal> Execute()
        {
            var result = new ControlFlowSignal();
            await UniTask.SwitchToThreadPool();
            Thread.Sleep(5000);
            await UniTask.SwitchToMainThread();

            
            return ControlFlowSignal.Down(Instruction.Choices[0].Response);
        }

        public override void Undo()
        {
        }
    }
}
