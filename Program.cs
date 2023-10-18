using FolderSync.AppStates;

namespace FolderSync
{
    internal class Program
    {


        static int Main(string[] args)
        {

            ProgramArgs programConfig;
            try
            {
                programConfig = new(args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error! Usage is program.exe <source folder> <destination folder> <interval (in seconds)> <logfile>");
                Console.WriteLine(ex.Message);
                return 1;
            }

            Console.Clear();

            AppStates.AppState? state = new WorkingState(programConfig);

            while (state != null)
            {
                state.SetUpConsole();
                state = state.Process();
                Thread.Sleep(500);
            }

            return 0;

        }




    }
}