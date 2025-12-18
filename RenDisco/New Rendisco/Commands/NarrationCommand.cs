using Cysharp.Threading.Tasks;

namespace RenDisco.Commands
{
    public class NarrationCommand : Command<Narration> 
    {
        public NarrationCommand(Narration instruction) : base(instruction)
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
