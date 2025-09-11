using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class IfConditionBlockCommand : Command<IfCondition>
    {
        public IfConditionBlockCommand(IfCondition instruction, SynchronizationContext synchronizationContext) : base(instruction, synchronizationContext)
        {
        }

        public override InstructionResult Execute()
        {
            return null;
        }

        public override void Undo()
        {

        }
    }
}
