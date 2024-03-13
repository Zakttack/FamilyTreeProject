using FamilyTreeLibrary.Data.Enumerators;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary
{
    public static partial class FamilyTreeUtils
    {
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
            string filePath = null;
            string[] relativeParts = relativeFilePath.Split('\\');
            IEnumerator<string> enumerator = new FileEnumerator(true);
            while(filePath is null && enumerator.MoveNext())
            {
                string[] parts = enumerator.Current.Split('\\');
                bool condition = (parts.Length == relativeParts.Length || (parts.Length > relativeParts.Length && parts[^relativeParts.Length] == relativeParts[0])) && parts.Intersect(relativeParts).Count() == relativeParts.Length;
                filePath = condition ? enumerator.Current : null;
            }
            enumerator.Dispose();
            return filePath;
        }

        public static IEnumerable<string> GetFilePathsOf(string fileName)
        {
            ICollection<string> filePaths = new List<string>();
            IEnumerator<string> enumerator = new FileEnumerator(true);
            while(enumerator.MoveNext())
            {
                if (enumerator.Current.Split('\\')[^1] == fileName)
                {
                    filePaths.Add(enumerator.Current);
                }
            }
            enumerator.Dispose();
            return filePaths;
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

        [GeneratedRegex(@"^\d+$")]
        internal static partial Regex NumberPattern();

        [GeneratedRegex(@"^\d+-\d+$")]
        internal static partial Regex RangePattern();
        public static string GetRootDirectory()
        {
            IEnumerator<string> enumerator = new FileEnumerator(false);
            while (enumerator.MoveNext())
            {
                FileInfo file = new(enumerator.Current);
                if (file.Directory.Name == "FamilyTreeProject")
                {
                    return file.DirectoryName;
                }
            }
            throw new InvalidOperationException("This isn't the FamilyTreeProject.");
        }
    }
}