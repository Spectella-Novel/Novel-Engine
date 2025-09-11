using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class JumpCommand : Command<Jump>
    {
        public JumpCommand(Jump instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }

        public override InstructionResult Execute()
        {
            var nextLabel = new Label();
            nextLabel.Name = Instruction.Label;
            var result = new InstructionResult();
            result.Next = nextLabel;
            return result;
        }

        public override void Undo()
        {
       
        }
    }
}
