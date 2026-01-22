using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class IfConditionBlockCommand : Command<IfCondition>
    {
        public IfConditionBlockCommand(IfCondition instruction, IStorage storage) : base(instruction)
        {
            Storage = storage;
        }

        public IStorage Storage { get; }

        public override async UniTask<ControlFlowSignal> Execute()
        {
            BooleanLiteral result = Instruction.Condition.Evaluate(Storage) as BooleanLiteral;
            
            if (result == null) throw new ArithmeticException("In condition must be boolean expression");

            if (result.Value)
            {
                return ControlFlowSignal.Down(Instruction.Content);
            }

            foreach (var elseblock in Instruction.ElifConditions) {
                result = elseblock.Condition.Evaluate(Storage) as BooleanLiteral;

                if (result == null) throw new ArithmeticException("In condition must be boolean expression");

                if (result.Value)
                {
                    return ControlFlowSignal.Down(elseblock.Content);
                }
            }
            return null;
        }

        public override void Undo()
        {

        }
    }
}
