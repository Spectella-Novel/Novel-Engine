
using Cysharp.Threading.Tasks;

namespace RenDisco.Commands
{
    public class DialogueCommand : Command<Dialogue>
    {
        public DialogueCommand(Dialogue instruction) : base(instruction)
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
