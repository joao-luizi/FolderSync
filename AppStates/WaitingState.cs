using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FolderSync.AppStates
{
    public class WaitingState : AppState
    {

        private bool SetUp;
        public WaitingState(ProgramArgs programConfig) : base(programConfig)
        {
            SetUp = false;
        }
        public override void SetUpConsole()
        {
            if (SetUp == false)
            {
                Console.Clear();
                Console.WriteLine($"Source Folder: {programConfig.sourceFolder}");             
                Console.WriteLine($"Destination Folder: {programConfig.destinationFolder}");          
                Console.WriteLine($"LogFile: {programConfig.logFilePath}");            
                Console.WriteLine($"Next synchronize: {programConfig.TimeToSync:mm\\:ss}");             
                Console.WriteLine("Press 'q' to terminate the application");
                SetUp = true;
            }
            else
            {
                int x = Console.GetCursorPosition().Left;
                int y = Console.GetCursorPosition().Top;
                Console.SetCursorPosition(0, 3);
                Console.Write($"Next synchronize: {programConfig.TimeToSync:mm\\:ss}");
                Console.SetCursorPosition(x, y);
            }
            
        }

        public override AppState? Process()
        {
            //User presses quit or it's time to start another sync task
            AppState? result = this;         
            if (Console.KeyAvailable)
            {
                var read = Console.ReadKey(true);
                if (read.Key == ConsoleKey.Q)
                {
                    return null;
                }
            }
            if (programConfig.TimeToSync < TimeSpan.Zero)
            {
                result = new WorkingState(programConfig);
            }
            return result;
        }

        
    }
}
