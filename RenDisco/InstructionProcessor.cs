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
        private Executor _executor;

        private IEnumerator<ControlFlowSignal> _currentSignal;
        private bool _running;
        private bool _init = false; 
        /// <summary>
        /// Constructor for the play execution environment.
        /// </summary>
        /// <param name="runtime">The engine that executes the script actions.</param>
        /// <param name="commands">List of commands to execute.</param>
        /// <param name="parent">The parent Play context, used for handling scopes and returns.</param>
        public InstructionProcessor(List<Instruction> commands, CommandFactory commandFactory)
        {

            _executor = new Executor(commandFactory);
            _running = true;

            _callStack = new Stack<InstructionContext>();
            _allCommands = commands;
        }


        public ControlFlowSignal GetNextControlSignal()
        {
            Init();

            if (!_currentSignal.MoveNext())
            {
               var instruction = GetNextInstruction();
                _currentSignal = getFlow(instruction);
            }

            return _currentSignal.Current;
        }

        private void Init()
        {
            if(_init) return;

            _instructionContext = new InstructionContext();
            _instructionContext.InstructionCounter = 0;
            _instructionContext.Instructions = _allCommands;

            var instruction = _instructionContext.Instructions[_instructionContext.InstructionCounter];

            _currentSignal = getFlow(instruction);


            _init = true;
        }




        /// <summary>
        /// Execute commands from the current ProgramCounter position.
        /// </summary>
        /// <param name="returnToParent">
        /// Specifies if this scope takes responsibility for calling parent context after completing current commands.
        /// </param>
        /// <param name="inputContext">Set our Step context.</param>
        /// <returns>Boolean indicating if execution should continue.</returns>
        public void Step()
        {
            if (!_running) return;

            Instruction nextInstruction = GetNextInstruction();

            if (nextInstruction == null) return;

            getFlow(nextInstruction);
        }
        public bool IsCompleted()
        {
            return _running;
        }
        /// <summary>
        /// Executes a single command.
        /// </summary>
        /// <param name="instruction">The command to execute.</param>
        private IEnumerator<ControlFlowSignal> getFlow(Instruction instruction)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException(nameof(instruction));
            }
            if (instruction is Return)
            {
                _running = false;
            }
            var flow = _executor.GetFlow(instruction).GetEnumerator();
            return flow;
        }

        private Instruction GetNextInstruction()
        {
            if (_instructionContext.Next != null)
            {
                var next = _instructionContext.Next;
                _instructionContext.Next = null; // Ñáðîñèòü jump ïîñëå èñïîëüçîâàíèÿ
                return next;
            }
            // Ïðè êîíöå èíñòðóêöèé ïåðåõîäèì íà ïðîøëûé óðîâåíü
            if (_instructionContext.InstructionCounter > _instructionContext.Instructions.Count)
            {
                if (_callStack.Count <= 0)
                {
                    _running = false;
                    return null;
                }
                var prevContext = _callStack.Pop();
                _instructionContext = prevContext;
            }

            return _instructionContext.Instructions[_instructionContext.InstructionCounter++];
        }

        private void HandleInstructionResult(ControlFlowSignal instructionResult)
        {
            if (instructionResult.Instructions != null)
            {
                // Ïåðåõîä ê ïîäïðîãðàììå
                _callStack.Push(_instructionContext);
                _instructionContext = new InstructionContext
                {
                    Instructions = instructionResult.Instructions,
                    InstructionCounter = 0
                };
            }
            else if (instructionResult.Next != null)
            {
                if (instructionResult.Next is Label label)
                {
                    // Ïåðåõîä ïî ìåòêå
                    _callStack.Clear();
                    var index = FindLabel(label.Name);
                    instructionResult.Next = _allCommands[index];
                }

                _instructionContext = new InstructionContext
                {
                    Next = instructionResult.Next,
                    InstructionCounter = 0
                };
            }
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
