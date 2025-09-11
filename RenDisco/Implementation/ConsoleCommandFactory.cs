using RenDisco.Commands;
using RenDisco.Implementation.Commands;
using System.Threading;


namespace RenDisco.Implementation
{
    public class ConsoleCommandFactory : CommandFactory
    {
        public ConsoleCommandFactory(IStorage storage) : base(storage){}

        protected override DefineCommand CreateDefineCommand(Define define)
        {
            return new ConsoleDefineCommand(define, Storage, SynchronizationContext);
        }

        protected override DialogueCommand CreateDialogueCommand(Dialogue dialogue)
        {
            return new ConsoleDialogueCommand(dialogue, SynchronizationContext);
        }

        protected override HideCommand CreateHideCommand(Hide hide)
        {
            return new ConsoleHideCommand(hide, SynchronizationContext);
        }

        protected override NarrationCommand CreateNarrationCommand(Narration narration)
        {
            return new ConsoleNarrationCommand(narration, SynchronizationContext);
        }

        protected override PauseCommand CreatePauseCommand(Pause pause)
        {
            return new ConsolePauseCommand(pause, SynchronizationContext);
        }

        protected override PlayMusicCommand CreatePlayMusicCommand(PlayMusic playMusic)
        {
            return new ConsolePlayMusicCommand(playMusic, SynchronizationContext);
        }

        protected override ShowImageCommand CreateShowImageCommand(Show show)
        {
            return new ConsoleShowImageCommand(show, SynchronizationContext);
        }

        protected override ShowSceneCommand CreateShowSceneCommand(Scene scene)
        {
            return new ConsoleShowSceneCommand(scene, SynchronizationContext);
        }

        protected override StopMusicCommand CreateStopMusicCommand(StopMusic stopMusic)
        {
            return new ConsoleStopMusicCommand(stopMusic, SynchronizationContext);
        }
    }
}
