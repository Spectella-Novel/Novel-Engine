using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RenDisco
{
    public class ControlFlowSignal
    {
        public enum Kind { Jump, Down, Continue }
        public virtual Kind Type => Kind.Continue;


        public sealed class JumpSignal : ControlFlowSignal
        {
            public string LabelName { get; }
            public override Kind Type => Kind.Jump;
            public JumpSignal(string labelName)
            {
                LabelName = labelName;
            }
        }

        public sealed class DownSignal : ControlFlowSignal
        {
            public IReadOnlyList<Instruction> Instructions { get; }
            public override Kind Type => Kind.Down;
            public DownSignal(IReadOnlyList<Instruction> instructions)
            {
                Instructions = instructions;
            }
        }

        // Фабрики
        public static ControlFlowSignal Jump(string label) => new JumpSignal(label);
        public static ControlFlowSignal Down(List<Instruction> insts) => new DownSignal(insts.AsReadOnly());
        public static ControlFlowSignal Continue() => new ControlFlowSignal();
    }
}
