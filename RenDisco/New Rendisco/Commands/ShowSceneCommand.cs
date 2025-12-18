using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenDisco.Commands
{
    public class ShowSceneCommand : Command<Scene>
    {
        public ShowSceneCommand(Scene instruction) : base(instruction)
        {
        }

        public override async UniTask<ControlFlowSignal> Execute()
        {
            return null;
        }

        public override void Undo()
        {
        }
    }
}
