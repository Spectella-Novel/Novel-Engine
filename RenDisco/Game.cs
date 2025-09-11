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
            Console.WriteLine(_running);
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
                    InstructionCounter = 0 // Начнем с -1, так как Step() увеличит до 0
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
        private int FindLabel(string labelName) // ToDo: реализовать линейный поиск по все лейблам
           
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

        /// <summary>
        /// Evaluate a boolean condition.
        /// </summary>
        /// <param name="condition">The condition as a string.</param>
        /// <returns>The result of the condition evaluation.</returns>
        //private bool EvaluateCondition(string condition)
        //{
        //    return ExpressionEvaluate.EvaluateCondition(_runtime, condition);
        //}


        ///// <summary>
        ///// Executes defined variables or character settings.
        ///// </summary>
        ///// <param name="define">The define command to execute.</param>
        //private void ExecuteDefine(Define define)
        //{
        //    _runtime.ExecuteDefine(define);
        //}

        ///// <summary>
        ///// Display a menu and handle choice consequences.
        ///// </summary>
        ///// <param name="menu">The menu command containing choices and responses.</param>
        ///// <returns>The result of the run, whether to break or not.</returns>
        //private void ExecuteMenu(Menu menu, InstructionContext? stepContext = null)
        //{
        //    try
        //    {
        //        if (stepContext?.Choice != null)
        //        {
        //            int selectedChoice = stepContext.Choice ?? -1;
        //            if (selectedChoice == -1) return;
        //            WaitingForInput = false;
        //            this._child = new Game(_runtime, menu.Choices[selectedChoice].Response, this);
        //            this._child.Step();
        //        }
        //        else if (!WaitingForInput)
        //        {
        //            _runtime.ShowChoices(menu.Choices);
        //            WaitingForInput = true;
        //        }
        //    }
        //    catch(Exception exception)
        //    {
        //        WaitingForInput = true;
        //        if (exception is ArgumentOutOfRangeException)
        //        {
        //            Console.WriteLine($"Unknown dialogue option: {stepContext?.Choice}");
        //            return;
        //        }

        //        Console.WriteLine($"Unhandled Exception: {exception.StackTrace}");
        //    }
        //}

        ///// <summary>
        ///// Handle commands conditionally.
        ///// </summary>
        ///// <param name="condition"></param>
        ///// <param name="content"></param>
        //private void ExecuteLabel(Label label, InstructionContext? stepContext = null)
        //{
        //    this._child = new Game(_runtime, label.Instrutions, this);
        //    this._child.Step();
        //}

        //private void ExecuteIfConditionalBlock(IfCondition block, InstructionContext? stepContext = null)
        //{
        //    // Execute the main if condition
        //    var result = ExecuteConditionalBlock(block.Condition, block.Content, stepContext);

        //    // Otherwise, let's iterate
        //    if (!result) {
        //        foreach (var ElifCondition in block.ElifConditions)
        //        {
        //            result = ExecuteConditionalBlock(ElifCondition.Condition, ElifCondition.Content, stepContext);
        //            if (result) break;
        //        }
        //    }

        //    // If the condition still isn't true, we go to our else.
        //    if (!result) {
        //        this._child = new Game(_runtime, block.ElseConditions.Content, this);
        //        this._child.Step();
        //    }
        //}

        ///// <summary>
        ///// Handle commands conditionally.
        ///// </summary>
        ///// <param name="condition"></param>
        ///// <param name="content"></param>
        ///// <returns>The result of the run, whether to break or not.</returns>
        //private bool ExecuteConditionalBlock(string condition, List<Instruction> content)
        //{
        //    if (EvaluateCondition(condition))
        //    {
        //        this._child = new Game(_runtime, content, this);
        //        this._child.Step();
        //        return true;
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// Execute a jump command which modifies the program counter.
        ///// </summary>
        ///// <param name="labelName">The label name to jump to.</param>
        //private void ExecuteJump(string labelName)
        //{
        //    //this._child = FindLabel(labelName);
        //    //if (this._child != null) {
        //    //    this._jumped = true;
        //    //    this._child.CurrentChild = null;
        //    //    this._child?.Step(returnToParent: true);
        //    //}
        //}  1 1 0 1 1 0 1 0 1 0 0 0 0 0 0 0 0 0 0 1 0 

    }
}