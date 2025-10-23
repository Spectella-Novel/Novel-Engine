using System.Collections.Generic;

namespace RenDisco.Commands
{
    public class JumpCommand : Command<Jump>
    {
        public JumpCommand(Jump instruction) : base(instruction)
        {
        }

        public override IEnumerable<ControlFlowSignal> Flow()
        {
            var nextLabel = new Label();
            nextLabel.Name = Instruction.Label;
            var result = new ControlFlowSignal();
            result.Next = nextLabel;
            yield return result;
        }

        public override void Undo()
        {
       
        }
    }
}
