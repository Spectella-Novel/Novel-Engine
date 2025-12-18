using System.Collections.Generic;

namespace RenDisco {
    public struct InstructionContext {
        public IReadOnlyList<Instruction> Instructions;
        public int InstructionCounter;
    }
}