using System.Collections.Generic;

namespace RenDisco {
    public class InstructionContext {
        public Instruction Next;
        public List<Instruction> Instructions = new();
        public int InstructionCounter = 0;
    }
}