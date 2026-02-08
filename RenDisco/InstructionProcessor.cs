using RenDisco.Commands;
using RenDisco.RuntimeException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenDisco
{
    internal class InstructionProcessor
    {
        private InstructionContext _instructionContext;
        private List<Instruction> _allCommands;
        private Stack<InstructionContext> _callStack;
        private bool _running;
        public int Line => _callStack.Select(ctx => ctx.InstructionCounter).Sum() + _instructionContext.InstructionCounter;
        /// <summary>
        /// Constructor for the play execution environment.
        /// </summary>
        /// <param name="runtime">The engine that executes the script actions.</param>
        /// <param name="commands">List of commands to execute.</param>
        /// <param name="parent">The parent Play context, used for handling scopes and returns.</param>
        public InstructionProcessor(List<Instruction> commands, CommandFactory commandFactory)
        {


            _callStack = new Stack<InstructionContext>();
            _allCommands = commands;
        }

        internal Instruction Start()
        {
            _instructionContext = new InstructionContext();
            _instructionContext.InstructionCounter = 0;
            _instructionContext.Instructions = _allCommands;
            _running = true;

            var instruction = _instructionContext.Instructions[_instructionContext.InstructionCounter];

            return instruction;
        }

        public Instruction ProcessNextInstruction(ControlFlowSignal controlSignal)
        {
            if (controlSignal == null)
                controlSignal = ControlFlowSignal.Continue();

            switch (controlSignal.Type)
            {
                case ControlFlowSignal.Kind.Jump:
                    if(controlSignal is ControlFlowSignal.JumpSignal jumpSignal)
                    {
                        var nextInstruction = FindLabel(jumpSignal.LabelName);
                        _callStack.Clear();
                        
                        _instructionContext.InstructionCounter = nextInstruction;
                        _instructionContext.Instructions = _allCommands;
                    }
                    break;
                case ControlFlowSignal.Kind.Down:
                    if (controlSignal is ControlFlowSignal.DownSignal downSignal)
                    {
                        _callStack.Push(_instructionContext);
                        
                        _instructionContext = new InstructionContext();

                        _instructionContext.InstructionCounter = 0;
                        _instructionContext.Instructions = downSignal.Instructions;
                    }
                    break;
                default:
                    if (_instructionContext.InstructionCounter + 1 >= _instructionContext.Instructions.Count)
                    {
                        if(_callStack.Count == 0) {
                            _running = false;
                            return null;
                        }
                        _instructionContext = _callStack.Pop();

                        goto default; //Checking the next instruction
                    }
                    _instructionContext.InstructionCounter++;
                    break;
            }
            return _instructionContext.Instructions[_instructionContext.InstructionCounter];
        }

        public bool IsRunning()
        {
            return _running;
        }

        ///// <summary>
        ///// Locate a label within the command set.
        ///// </summary>
        ///// <param name="labelName">The name of the label to find.</param>
        ///// <returns>The Play instance associated with the found label, or null if no label found.</returns>
        private int FindLabel(string labelName)
        {
            for (var i = 0; i < _allCommands.Count; i++)
            {
                if (_allCommands[i] is Label label && label.Name == labelName)
                {
                    return i;
                }
            }
            throw new LabelNotImplementedException($"{labelName}");
        }

    }
}
