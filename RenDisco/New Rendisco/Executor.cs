using RenDisco.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace RenDisco
{
    internal class Executor
    {
        private readonly CommandFactory _factory;

        public Executor(CommandFactory commandFactory)
        {
            _factory = commandFactory;
        }

    }
}
