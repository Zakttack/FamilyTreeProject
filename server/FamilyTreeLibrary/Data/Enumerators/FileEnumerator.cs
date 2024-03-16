using FamilyTreeLibrary.Data.Comparers;
using System.Collections;

namespace FamilyTreeLibrary.Data.Enumerators
{
    public class FileEnumerator : IEnumerator<string>
    {
        private const string ROOT_DIRECTORY = @"C:\FamilyTreeProject";
        private PriorityQueue<Queue<string>,DirectoryInfo> filePathsCollection;
        private readonly bool loggerInitialized;
        private string currentFilePath;

        public FileEnumerator(bool loggerInitialized)
        {
            this.loggerInitialized = loggerInitialized;
            Reset();
        }

        public string Current
        {
            get
            {
                return currentFilePath;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return currentFilePath;
            }
        }

        public void Dispose()
        {
            filePathsCollection = null;
            currentFilePath = null;
        }

        public bool MoveNext()
        {
            while(filePathsCollection.TryDequeue(out Queue<string> filePaths, out DirectoryInfo current))
            {
                try
                {
                    if (filePaths.TryDequeue(out currentFilePath))
                    {
                        filePathsCollection.Enqueue(filePaths, current);
                        return true;
                    }
                    IEnumerable<DirectoryInfo> subDirectories = current.EnumerateDirectories();
                    foreach (DirectoryInfo subDirectory in subDirectories)
                    {
                        filePathsCollection.Enqueue(GetFilePaths(subDirectory, loggerInitialized), subDirectory);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    if (loggerInitialized)
                    {
                        FamilyTreeUtils.LogMessage(LoggingLevels.Warning, ex.Message);
                    }
                }
            }
            return false;
        }

        public void Reset()
        {
            filePathsCollection = new(new DirectoryComparer());
            DirectoryInfo root = new(ROOT_DIRECTORY);
            Queue<string> filePaths = GetFilePaths(root, loggerInitialized);
            filePathsCollection.Enqueue(filePaths, root);
        }

        private static Queue<string> GetFilePaths(DirectoryInfo directory, bool loggerInitialized)
        {
            Queue<string> filePathsQueue = new();
            try
            {
                IEnumerable<FileInfo> files = directory.EnumerateFiles();
                foreach (FileInfo file in files)
                {
                    try
                    {
                        filePathsQueue.Enqueue(file.FullName);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        if (loggerInitialized)
                        {
                            FamilyTreeUtils.LogMessage(LoggingLevels.Warning, ex.Message);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                if (loggerInitialized)
                {
                    FamilyTreeUtils.LogMessage(LoggingLevels.Warning, ex.Message);
                }
            }
            return filePathsQueue;
        }
    }
}