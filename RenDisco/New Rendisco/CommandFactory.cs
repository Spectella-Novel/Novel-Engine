


using System;
using System.Collections.Generic;
using System.Threading;

namespace RenDisco.Commands
{
    public abstract class CommandFactory
    {
        protected IStorage Storage;
        protected  SynchronizationContext SynchronizationContext;
        private bool IsInit = false;

        protected CommandFactory(IStorage storage)
        {
            Storage = storage;
        }

        public void InitContext(SynchronizationContext synchronizationContext)
        {
            SynchronizationContext = synchronizationContext;
            IsInit = true;
        }

        public virtual Command CreateCommand(RenDisco.Instruction command)
        {
            if(!IsInit) { return null; }
            Console.WriteLine(command.ToString());
            switch (command)
            {
                case Label label:
                    return CreateLabelCommand(label);
                case Dialogue dialogue:
                    return CreateDialogueCommand(dialogue);
                case Narration narration:
                    return CreateNarrationCommand(narration);
                case Scene scene:
                    return CreateShowSceneCommand(scene);
                case Show show:
                    return CreateShowImageCommand(show);
                case Define define:
                    return CreateDefineCommand(define);
                case IfCondition ifCondition:
                    return CreateIfConditionBlockCommand(ifCondition);
                case ElifCondition elifCondition:
                    return CreateElIfConditionalBlockCommand(elifCondition);
                case Menu menu:
                    return CreateExecuteMenuCommand(menu);
                case Jump jump:
                    return CreateExecuteJumpCommand(jump);
                case PlayMusic playMusic:
                    return CreatePlayMusicCommand(playMusic);
                case Pause pause:
                    return CreatePauseCommand(pause);
                case StopMusic stopMusic:
                    return CreateStopMusicCommand(stopMusic);
                case Hide hide:
                    return CreateHideCommand(hide);
                default:
                    Console.WriteLine($"Unknown command type encountered: {command.Type}");
                    break;
            }
            return null;
        }

        protected virtual LabelCommand CreateLabelCommand(Label label)
        {
            return new LabelCommand(label, SynchronizationContext);
        }

        protected virtual JumpCommand CreateExecuteJumpCommand(Jump jump)
        {
            return new JumpCommand(jump, SynchronizationContext);
        }

        protected virtual MenuCommand CreateExecuteMenuCommand(Menu menu)
        {
            return new MenuCommand(menu, SynchronizationContext);
        }

        protected virtual ElIfConditionalBlockCommand CreateElIfConditionalBlockCommand(ElifCondition elifCondition)
        {
            return new ElIfConditionalBlockCommand(elifCondition, SynchronizationContext);
        }

        protected virtual IfConditionBlockCommand CreateIfConditionBlockCommand(IfCondition ifCondition)
        {
            return new IfConditionBlockCommand(ifCondition, SynchronizationContext);
        }

        protected abstract DefineCommand CreateDefineCommand(Define define);
        protected abstract ShowImageCommand CreateShowImageCommand(Show show);
        protected abstract ShowSceneCommand CreateShowSceneCommand(Scene scene);
        protected abstract NarrationCommand CreateNarrationCommand(Narration narration);
        protected abstract DialogueCommand CreateDialogueCommand(Dialogue dialogue);
        protected abstract HideCommand CreateHideCommand(Hide hide);
        protected abstract StopMusicCommand CreateStopMusicCommand(StopMusic stopMusic);
        protected abstract PauseCommand CreatePauseCommand(Pause pause);
        protected abstract PlayMusicCommand CreatePlayMusicCommand(PlayMusic playMusic);
    }
}
