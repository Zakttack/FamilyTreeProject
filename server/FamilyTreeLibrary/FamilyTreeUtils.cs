using FamilyTreeLibrary.Data;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary
{
    public static partial class FamilyTreeUtils
    {
        public static IEnumerable<string> FilePaths
        {
            get
            {
                return new FileEnumerable();
            }
        }
        public static IConfiguration GetConfiguration(string appSettingsFilePath)
        {
            if (appSettingsFilePath == null || !File.Exists(appSettingsFilePath))
            {
                throw new FileNotFoundException("Configuration file isn't found.");
            }
            string[] filePathChecker = appSettingsFilePath.Split('.');
            if (filePathChecker[^1] != "json")
            {
                throw new NotSupportedException("App Settings Configuration must be stored as a json file.");
            }
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(appSettingsFilePath))
                .AddJsonFile(Path.GetFileName(appSettingsFilePath), false, true);
            return builder.Build();
        }

        public static string GetFilePathOf(string relativeFilePath)
        {
            string[] relativeParts = relativeFilePath.Split('\\');
            return FilePaths.Where((filePath) => 
            {
                string[] parts = filePath.Split('\\');
                string[] intersect = parts.Intersect(relativeParts).ToArray();
                return relativeFilePath == string.Join('\\', intersect);
            }).FirstOrDefault();
        }

        public static IEnumerable<string> GetFilePathsOf(string fileName)
        {
            return FilePaths.Where((filePath) => 
            {
                string[] parts = filePath.Split('\\');
                return fileName == parts[^1];
            });
        }

        public static void InitializeLogger()
        {
            string filePath = Path.Combine(GetRootDirectory(),@"resources\Logs\log.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static void LogMessage(LoggingLevels level, string message)
        {
            switch (level)
            {
                case LoggingLevels.Debug: Log.Debug(message); break;
                case LoggingLevels.Information: Log.Information(message); break;
                case LoggingLevels.Warning: Log.Warning(message); break;
                case LoggingLevels.Error: Log.Error(message); break;
                case LoggingLevels.Fatal: Log.Fatal(message); break;
            }
        }

        public static void WriteError(Exception ex)
        {
            Log.Fatal($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }

        [GeneratedRegex(@"^\d+$")]
        internal static partial Regex NumberPattern();

        [GeneratedRegex(@"^\d+-\d+$")]
        internal static partial Regex RangePattern();
        private static string GetRootDirectory()
        {
            Stack<DirectoryInfo> directories = new();
            string targetDirectoryName = "FamilyTreeProject";
            directories.Push(new DirectoryInfo(Directory.GetCurrentDirectory()));
            while (directories.TryPop(out DirectoryInfo current))
            {
                try
                {
                    if (current.Name == targetDirectoryName)
                    {
                        return current.FullName;
                    }
                    directories.Push(current.Parent);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
            }
            throw new InvalidOperationException("This isn't the FamilyTreeProject.");
        }
    }
}