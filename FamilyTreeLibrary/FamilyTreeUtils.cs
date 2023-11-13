using FamilyTreeLibrary.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FamilyTreeLibrary
{
    public static class FamilyTreeUtils
    {
        internal const string NUMBER_PATTERN = @"^\d+$";
        internal const string RANGE_PATTERN = @"^\d+-\d+$";
        private const string LOG_FILE_NAME = @"Logs\log.txt";

        public static FamilyTreeDate DefaultDate
        {
            get
            {
                return new()
                {
                    Year = "",
                    Month = "",
                    Day = 0
                };
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

        public static string GetFileNameFromResources(string currentPath, string fileNameWithExtension)
        {
            string[] parts = currentPath.Split('\\');
            string current = parts[^1];
            if (current != "FamilyTreeProject")
            {
                return GetFileNameFromResources(currentPath[..(currentPath.Length - current.Length - 1)], fileNameWithExtension);
            }
            return $@"{currentPath}\Resources\{fileNameWithExtension}";
        }

        public static void InitializeLogger()
        {
                string filePath = GetFileNameFromResources(Directory.GetCurrentDirectory(), LOG_FILE_NAME);
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
        }
    }
}