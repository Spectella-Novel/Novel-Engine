using Cysharp.Threading.Tasks;
using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace RenDisco {
    public class GameFlow
    {
        private UniTask<UniTaskVoid> workflowTask;
        private bool _running;
        private CancellationTokenSource _cancellationTokenSource;
        private SynchronizationContext _syncContext;
        /// <summary>
        /// Constructor for the play execution environment.
        /// </summary>
        /// <param name="runtime">The engine that executes the script actions.</param>
        /// <param name="commands">List of commands to execute.</param>
        /// <param name="parent">The parent Play context, used for handling scopes and returns.</param>
        public GameFlow(List<Instruction> commands, CommandFactory commandFactory)
        {
            var commandsClone = commands.ToList();
            _instructionContext = new InstructionContext();
            _instructionContext.InstructionCounter = 0;
            _instructionContext.Instructions = commandsClone;
            _factory = commandFactory;

            Parents = new Stack<InstructionContext>();
            Commands = commandsClone;
            _instructionProcessor = new InstructionProcessor(Commands, commandFactory);

            _cancellationTokenSource = new CancellationTokenSource();

        }

        public void Start()
        {
            if (_running)  return;
            Workflow().Forget();
            _running = true;
        }

        public void Stop()
        {
            _running = false;
            _cancellationTokenSource.Cancel();
        }

        //Ниже идет отдельный поток
        private InstructionContext _instructionContext;
        public List<Instruction> Commands;
        private InstructionProcessor _instructionProcessor;
        private CommandFactory _factory;
        public Stack<InstructionContext> Parents;

        public async UniTaskVoid Workflow()
        {
            try
            {
                var instructions = _instructionProcessor.Start();
                while (_instructionProcessor.IsRunning() && instructions != null)
                {
                    var command = _factory.CreateCommand(instructions);
                    var controlSignal = await command.Execute();
                    instructions = _instructionProcessor.ProcessNextInstruction(controlSignal);
                }

                if (instructions == null && _instructionProcessor.IsRunning())
                {
                    Debug.LogError("Unexpected behavior instruction equals null");
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Ошибка в строке: " + _instructionProcessor.Line + '\n' + ex.Message);
            }


        }
    }
}