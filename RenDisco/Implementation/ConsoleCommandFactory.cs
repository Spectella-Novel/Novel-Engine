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
            return new ConsoleDefineCommand(define, Storage);
        }

        protected override DialogueCommand CreateDialogueCommand(Dialogue dialogue)
        {
            return new ConsoleDialogueCommand(dialogue);
        }

        protected override HideCommand CreateHideCommand(Hide hide)
        {
            return new ConsoleHideCommand(hide);
        }

        protected override NarrationCommand CreateNarrationCommand(Narration narration)
        {
            return new ConsoleNarrationCommand(narration);
        }

        protected override PauseCommand CreatePauseCommand(Pause pause)
        {
            return new ConsolePauseCommand(pause);
        }

        protected override PlayMusicCommand CreatePlayMusicCommand(PlayMusic playMusic)
        {
            return new ConsolePlayMusicCommand(playMusic);
        }

        protected override ShowImageCommand CreateShowImageCommand(Show show)
        {
            return new ConsoleShowImageCommand(show);
        }

        protected override ShowSceneCommand CreateShowSceneCommand(Scene scene)
        {
            return new ConsoleShowSceneCommand(scene);
        }

        protected override StopMusicCommand CreateStopMusicCommand(StopMusic stopMusic)
        {
            return new ConsoleStopMusicCommand(stopMusic);
        }
    }
}
