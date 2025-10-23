using RenDisco.Commands;
using RenDisco.RuntimeException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;


namespace RenDisco {
    public class GameFlow
    {
      
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
            _running = true;

            Parents = new Stack<InstructionContext>();
            Commands = commandsClone;

            _controlFlowProcessor = new ControlFlowProcessor();

        }
        
        public void Start()
        {
            if (_running)  return; 
            
            _syncContext = SynchronizationContext.Current;

            _controlFlowProcessor.Register<ContinueInInputThread>(new ContinueInInputThreadHandler(_syncContext)); // условно синхронный обработчик


            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(Workflow, _cancellationTokenSource.Token);
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
        private ControlFlowProcessor _controlFlowProcessor;
        private CommandFactory _factory;
        public Stack<InstructionContext> Parents;

        public void Workflow()
        {
            var instructions = _instructionContext.Instructions[_instructionContext.InstructionCounter];

            var command = _factory.CreateCommand(instructions);

            IEnumerator<ControlFlowSignal> commandFlow = command.Flow().GetEnumerator();
            var canNext = commandFlow.MoveNext();
            while (canNext)
            {
                var signal = commandFlow.Current;

                _controlFlowProcessor.Process(signal, () =>
                {
                    canNext = commandFlow.MoveNext();
                });
            }
        }

 
    }
}