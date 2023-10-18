namespace FolderSync.AppStates
{
    public abstract class AppState
    {
        public ProgramArgs programConfig;

        public AppState(ProgramArgs programConfig)
        {
            this.programConfig = programConfig;
        }
        public abstract void SetUpConsole();

        public abstract AppState? Process();


    }
}
