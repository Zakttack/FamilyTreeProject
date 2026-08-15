using Azure.Identity;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace VirtualFamilyMuseumLibrary
{
    public static class Extensions
    {
        public const char EN_DASH = '\u2013';
        public static IConfigurationBuilder AddConstantStorePipeline(this IConfigurationBuilder builder, ExecutionTypes executionType)
        {
            builder.AddEnvironmentVariables();
            string? appConfigurationEndpoint = executionType switch
            {
                ExecutionTypes.API => Environment.GetEnvironmentVariable("FamilyConfiguration__Endpoint"),
                ExecutionTypes.Console => Environment.GetEnvironmentVariable("FamilyConfiguration__Endpoint"),
                ExecutionTypes.Functions => Environment.GetEnvironmentVariable("FamilyConfiguration__Endpoint"),
                ExecutionTypes.Test => Environment.GetEnvironmentVariable("FamilyConfigurationTest__Endpoint"),
                _ => null,
            };
            if (!string.IsNullOrWhiteSpace(appConfigurationEndpoint))
            {
                builder.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(appConfigurationEndpoint), new DefaultAzureCredential());
                    options.Select(KeyFilter.Any);
                });
            }
            string? keyVaultEndpoint = executionType switch
            {
                ExecutionTypes.API => Environment.GetEnvironmentVariable("FamilyVault__Endpoint"),
                ExecutionTypes.Console => Environment.GetEnvironmentVariable("FamilyVault__Endpoint"),
                ExecutionTypes.Functions => Environment.GetEnvironmentVariable("FamilyVault__Endpoint"),
                ExecutionTypes.Test => Environment.GetEnvironmentVariable("FamilyVaultTest__Endpoint"),
                _ => null
            };
            if (!string.IsNullOrWhiteSpace(keyVaultEndpoint))
            {
                builder.AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential());
            }
            return builder;
        }

        public static IHostApplicationBuilder AddFamilyInsights(this IHostApplicationBuilder builder, ExecutionTypes executionType)
        {
            string? connectionString = builder.Configuration["FamilyInsights:ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return builder;
            }

            builder.Logging.SetMinimumLevel(LogLevel.Debug);

            builder.Services.AddSingleton<ITelemetryInitializer>(new RoleNameTelemetryInitializer(executionType.ToString()));

            builder.Services.AddApplicationInsightsTelemetryWorkerService(options =>
            {
                options.ConnectionString = connectionString;
            });

            builder.Logging.AddApplicationInsights(
                config => config.ConnectionString = connectionString,
                options => { });

            builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(null, LogLevel.Debug);

            return builder;
        }

        private sealed class RoleNameTelemetryInitializer(string roleName) : ITelemetryInitializer
        {
            public void Initialize(ITelemetry telemetry)
            {
                telemetry.Context.Cloud.RoleName = roleName;
            }
        }
    }
}