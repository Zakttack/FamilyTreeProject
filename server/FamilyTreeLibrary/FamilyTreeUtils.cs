﻿using Microsoft.Extensions.Configuration;
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
            return Path.Combine(GetRootDirectory(), relativeFilePath);
        }

        public static void InitializeLogger()
        {
            string filePath = GetFilePathOf(@"resources\Logs\log.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
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
            DirectoryInfo directory = new(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetDirectories(".git").Any())
            {
                directory = directory.Parent;
            }
            return directory.FullName;
        }
    }
}