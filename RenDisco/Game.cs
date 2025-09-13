using RenDisco.Commands;
using RenDisco.RuntimeException;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;


namespace RenDisco {
    public class Game
    {
        private InstructionContext _instructionContext;
      
        public List<Instruction> Commands;
        public Stack<InstructionContext> Parents;
        private bool _running;

        Executor _executor;

        /// <summary>
        /// Constructor for the play execution environment.
        /// </summary>
        /// <param name="runtime">The engine that executes the script actions.</param>
        /// <param name="commands">List of commands to execute.</param>
        /// <param name="parent">The parent Play context, used for handling scopes and returns.</param>
        public Game(List<Instruction> commands, CommandFactory commandFactory)
        {
            _instructionContext = new InstructionContext();
            _instructionContext.InstructionCounter = 0;
            _instructionContext.Instructions = commands;
            _executor = new Executor(commandFactory);
            _running = true;

            Parents = new Stack<InstructionContext>();
            Commands = commands;
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

            ExecuteCommand(nextInstruction);
        }
        public bool IsCompleted()
        {
            return _running;
        }
        /// <summary>
        /// Executes a single command.
        /// </summary>
        /// <param name="instruction">The command to execute.</param>
        private void ExecuteCommand(Instruction instruction)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException(nameof(instruction));
            }
            if(instruction is Return)
            {
                _running = false;
            }
            var instructionResult = _executor.ExecuteCommand(instruction);

            if (instructionResult == null)
            {
                return;
            }

            HandleInstructionResult(instructionResult);
        }

        private Instruction GetNextInstruction()
        {
            if (_instructionContext.Next != null)
            {
                var next = _instructionContext.Next;
                _instructionContext.Next = null; // Сбросить jump после использования
                return next;
            }
            // При конце инструкций переходим на прошлый уровень
            if (_instructionContext.InstructionCounter > _instructionContext.Instructions.Count) 
            {
                if(Parents.Count <= 0) 
                {
                    _running = false;
                    return null;
                }
                var prevContext = Parents.Pop();
                _instructionContext = prevContext;
            }

            return _instructionContext.Instructions[_instructionContext.InstructionCounter++];
        }

        private void HandleInstructionResult(InstructionResult instructionResult)
        {
            if (instructionResult.Instructions != null)
            {
                // Переход к подпрограмме
                Parents.Push(_instructionContext);
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
                    // Переход по метке
                    Parents.Clear();
                    var index = FindLabel(label.Name);
                    instructionResult.Next = Commands[index];
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
            for (var i = 0; i < Commands.Count; i++)
            {
                if (Commands[i] is Label label && label.Name == labelName)
                {
                    return i;
                }
            }
            throw new LabelNotImplementedException($"{labelName}" );
        }
    }
}