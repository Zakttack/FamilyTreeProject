using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace FamilyTreeLibrary
{
    public static class FamilyTreeUtils
    {
        private static ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        public static ILogger<T> CreateLogger<T>()
        {
            return loggerFactory.CreateLogger<T>();
        }
    }
}