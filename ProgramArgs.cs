namespace FolderSync
{
    public class ProgramArgs
    {
        private DateTime ScheduledSyncTime;


        public string sourceFolder;
        public string destinationFolder;
        public int checkInterval;
        public string logFilePath;



        public ProgramArgs(string[] args)
        {
            if (args.Length < 4)
            {
                throw new ArgumentException("Insuficient parameters provided.");
            }


            sourceFolder = args[0];

            if (!Directory.Exists(sourceFolder))
            {
                throw new ArgumentException($"Invalid source folder: {sourceFolder}");
            }

            destinationFolder = args[1];

            if (!Directory.Exists(destinationFolder))
            {
                throw new ArgumentException($"Invalid destination folder: {destinationFolder}");
            }

            if (!int.TryParse(args[2], out checkInterval))
            {
                throw new ArgumentException("Interval parameter must be an integer.");
            }
            ResetSyncTime();

            logFilePath = args[3];

            if (!File.Exists(logFilePath))
            {
                throw new ArgumentException($"Invalid log file:{logFilePath}");
            }


        }
        public void ResetSyncTime()
        {
            ScheduledSyncTime = DateTime.Now.AddSeconds(checkInterval);
        }

        public TimeSpan TimeToSync
        {
            get
            {
                return ScheduledSyncTime - DateTime.Now;
            }
        }
    }
}
