


using System;
using System.Collections.Generic;
using System.Threading;

namespace RenDisco.Commands
{
    public abstract class CommandFactory
    {
        protected IStorage Storage;

        protected CommandFactory(IStorage storage)
        {
            Storage = storage;
        }


        public virtual Command CreateCommand(RenDisco.Instruction command)
        {
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
            return new LabelCommand(label);
        }

        protected virtual JumpCommand CreateExecuteJumpCommand(Jump jump)
        {
            return new JumpCommand(jump);
        }

        protected virtual MenuCommand CreateExecuteMenuCommand(Menu menu)
        {
            return new MenuCommand(menu);
        }

        protected virtual ElIfConditionalBlockCommand CreateElIfConditionalBlockCommand(ElifCondition elifCondition)
        {
            return new ElIfConditionalBlockCommand(elifCondition);
        }

        protected virtual IfConditionBlockCommand CreateIfConditionBlockCommand(IfCondition ifCondition)
        {
            return new IfConditionBlockCommand(ifCondition);
        }

        protected virtual DefineCommand CreateDefineCommand(Define define) 
        {
            return new DefineCommand(define, Storage);
        }

        protected virtual ShowImageCommand CreateShowImageCommand(Show show)
        {
            return new ShowImageCommand(show);
        }

        protected virtual ShowSceneCommand CreateShowSceneCommand(Scene scene)
        {
            return new ShowSceneCommand(scene);
        }
        protected virtual NarrationCommand CreateNarrationCommand(Narration narration)
        {
            return new NarrationCommand(narration);
        }

        protected virtual DialogueCommand CreateDialogueCommand(Dialogue dialogue)
        {
            return new DialogueCommand(dialogue);
        }

        protected virtual HideCommand CreateHideCommand(Hide hide)
        {
            return new HideCommand(hide);
        }

        protected virtual StopMusicCommand CreateStopMusicCommand(StopMusic stopMusic)
        {
            return new StopMusicCommand(stopMusic);
        }

        protected virtual PauseCommand CreatePauseCommand(Pause pause)
        {
            return new PauseCommand(pause);
        }

        protected virtual PlayMusicCommand CreatePlayMusicCommand(PlayMusic playMusic)
        {
            return new PlayMusicCommand(playMusic);
        }
    }
}
