using System.Text;

namespace FolderSync.AppStates
{
    public class WorkingState : AppState
    {
        private StringBuilder LogMessages;

        public WorkingState(ProgramArgs programConfig) : base(programConfig)
        {
            LogMessages = new();
        }

        public override void SetUpConsole()
        {
            Console.Clear();
            LogMessage("Synchronization started");
        }

        public override AppState? Process()
        {
            //IEnumerable<string> correct = Directory.GetDirectories(programConfig.sourceFolder, "*.*", SearchOption.AllDirectories).Select(x => x.Replace(programConfig.sourceFolder + "\\", ""));
            IEnumerable<string> sourceDirsPath = Directory.GetDirectories(programConfig.sourceFolder, "*.*", SearchOption.AllDirectories).Select(x => Path.GetRelativePath(programConfig.sourceFolder, x));
            IEnumerable<string> destDirsPath = Directory.GetDirectories(programConfig.destinationFolder, "*.*", SearchOption.AllDirectories).Select(x => Path.GetRelativePath(programConfig.destinationFolder, x));

            bool areIdenticalDirs = sourceDirsPath.SequenceEqual(destDirsPath);

            if (!areIdenticalDirs)
            {
                ProcessDirsInSource(sourceDirsPath, destDirsPath);

                ProcessDirsInDest(sourceDirsPath, destDirsPath);
            }

            IEnumerable<string> sourcefilesPath = Directory.GetFiles(programConfig.sourceFolder, "*.*", SearchOption.AllDirectories).Select(x => Path.GetRelativePath(programConfig.sourceFolder, x));
            IEnumerable<string> destfilesPath = Directory.GetFiles(programConfig.destinationFolder, "*.*", SearchOption.AllDirectories).Select(x => Path.GetRelativePath(programConfig.destinationFolder, x));
            bool areIdenticalFiles = sourcefilesPath.SequenceEqual(destfilesPath);

            if (!areIdenticalFiles)
            {
                FileCompare myFileCompare = new FileCompare();

                ProcessFilesInSourceNotInDest(sourcefilesPath, destfilesPath, myFileCompare);

                ProcessFilesInSourceAndInDest(sourcefilesPath, destfilesPath, myFileCompare);

                ProcessFilesInDestNotInSource(sourcefilesPath, destfilesPath);

            }

            if (areIdenticalFiles && areIdenticalDirs)
            {
                LogMessage("Folders are synchronized");
                LogMessage("Exiting");
            }


            DumpLog();
            Thread.Sleep(3000);
            GC.Collect();
            programConfig.ResetSyncTime();
            return new WaitingState(programConfig);
        }

        private void ProcessDirsInSource(IEnumerable<string> sourceDirsPath, IEnumerable<string> destDirsPath)
        {
            //Dirs in Source and not found in Destination - Create Path even devoid of files for 100% sync
            var exceptDirsInSource = sourceDirsPath.Except(destDirsPath);
            LogMessage($"Found {exceptDirsInSource.Count()} folders missing in destination");
            foreach (string s in exceptDirsInSource)
            {
                string destinationDir = Path.Combine(programConfig.destinationFolder, s);
                string message = "";
                try
                {
                    Directory.CreateDirectory(destinationDir);
                    message += $"Folder: {destinationDir} created successfully";
                }
                catch (Exception ex)
                {
                    message += $"Creation of folder: {destinationDir} failed: {ex.Message}";
                }
                LogMessage(message);
            }
        }

        private void ProcessDirsInDest(IEnumerable<string> sourceDirsPath, IEnumerable<string> destDirsPath)
        {
            //Dirs in Dest and not in Source - Delete recursively
            var exceptDirsOut = destDirsPath.Except(sourceDirsPath);
            LogMessage($"Found {exceptDirsOut.Count()} folders in destination not found in source.");
            foreach (string s in exceptDirsOut)
            {
                string destinationDir = Path.Combine(programConfig.destinationFolder, s);
                string message = "";
                try
                {
                    Directory.Delete(destinationDir, true);
                    message += $"Folder: {destinationDir} deleted successfully";
                }
                catch (Exception ex)
                {
                    message += $"Deletion of folder: {destinationDir} failed: {ex.Message}";
                }
                LogMessage(message);
            }
        }

        private void ProcessFilesInSourceNotInDest(IEnumerable<string> sourcefilesPath, IEnumerable<string> destfilesPath, FileCompare fileCompare)
        {
            //Files in Source and not in Destination - Do Copy & CheckSum
            var exceptInSource = sourcefilesPath.Except(destfilesPath);
            LogMessage($"Found {exceptInSource.Count()} files in Source not found in Destination.");
            foreach (string s in exceptInSource)
            {
                string message = "";
                string source = Path.Combine(programConfig.sourceFolder, s);
                string destination = Path.Combine(programConfig.destinationFolder, s);
                string destinationDir = Path.GetDirectoryName(destination);
                try
                {
                    //This call to CreateDirectory is safe although probably not necessary
                    Directory.CreateDirectory(destinationDir);
                    File.Copy(source, destination, true);
                    message += $"File: {source} copied. Comparing...";
                    message += fileCompare.Equals(new FileInfo(source), new FileInfo(destination)) ? " OK." : "Failed.";
                }
                catch (Exception ex)
                {
                    message += $"Copy of file: {source} to {destination} failed: {ex.Message}";
                }
                LogMessage(message);
            }
        }

        private void ProcessFilesInSourceAndInDest(IEnumerable<string> sourcefilesPath, IEnumerable<string> destfilesPath, FileCompare fileCompare)
        {
            //Files in Source and in Destination - CheckSum for equality, Copy if not equal, check again and report sucess or fail
            var intersectInSource = sourcefilesPath.Intersect(destfilesPath);
            LogMessage($"Found {intersectInSource.Count()} files in Source and in Destination.");
            foreach (string s in intersectInSource)
            {
                string message = "";
                string source = Path.Combine(programConfig.sourceFolder, s);
                string destination = Path.Combine(programConfig.destinationFolder, s);
                if (!fileCompare.Equals(new FileInfo(source), new FileInfo(destination)))
                {
                    message += $"File {source} not equal to {destination} overwriting...";
                    try
                    {
                        File.Copy(source, destination, true);
                        message += $"File: {source} copied. Comparing...";
                        message += fileCompare.Equals(new FileInfo(source), new FileInfo(destination)) ? " OK." : "Failed.";
                    }
                    catch (Exception ex)
                    {
                        message += $"Copy of file: {source} to {destination} failed: {ex.Message}";
                    }
                }
                else
                {
                    message += $"File {source} equal to {destination}.";
                }
                LogMessage(message);
            }
        }

        private void ProcessFilesInDestNotInSource(IEnumerable<string> sourcefilesPath, IEnumerable<string> destfilesPath)
        {
            //Files in Dest and not in Source - Delete
            var exceptOut = destfilesPath.Except(sourcefilesPath);
            LogMessage($"Found {exceptOut.Count()} files in Destination not in Source.");
            foreach (string s in exceptOut)
            {
                string message = "";
                string destination = Path.Combine(programConfig.destinationFolder, s);
                try
                {
                    File.Delete(destination);
                    message += $"File: {destination} deleted successfully";
                }
                catch (Exception ex)
                {
                    message += $"Deletion of file: {destination} failed: {ex.Message}";
                }
                LogMessage(message);

            }
        }



        private void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
            LogMessages.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}{Environment.NewLine}");
        }

        private void DumpLog()
        {
            try
            {
                System.IO.File.WriteAllText(programConfig.logFilePath, LogMessages.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ocurred while writing logfile ({programConfig.logFilePath}): {ex.Message}");
            }

        }
    }
}
