using Cysharp.Threading.Tasks;

namespace RenDisco.Commands
{
    public  class PauseCommand : Command<Pause>
    {
        public PauseCommand(Pause instruction) : base(instruction)
        {
        }

        public override async UniTask<ControlFlowSignal> Execute()
        {
            await UniTask.WaitForSeconds((float)Instruction.Duration);
            return null;
        }

        public override void Undo()
        {
        }
    }
}
