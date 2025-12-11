namespace VirtualFamilyMuseumLibrary;
using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualFamilyMuseumLibrary.ConstantStore;
using VirtualFamilyMuseumLibrary.ConstantStore.Models;
using VirtualFamilyMuseumLibrary.ConstantStore.Repositories;

public static class FamilyUtils
{
    public static INonSensitiveConstantRepository? NonSensitiveConstantRepository
    {
        get;
        private set;
    }

    public static ISensitiveConstantRepository? SensitiveConstantRepository
    {
        get;
        private set;
    }

    public static ConstantStoreService? Service
    {
        get;
        private set;
    }
    public static IHostApplicationBuilder AddFamilyConfiguration(this IHostApplicationBuilder builder, bool allowLocalJsonFallback = true)
    {
        const string BOOTSTRAP_KEY = "FAMILY_CONFIGURATION_URI";
        builder.Configuration.AddEnvironmentVariables();
        if (allowLocalJsonFallback)
        {
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
        }
        string? endpoint = builder.Configuration[BOOTSTRAP_KEY];
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException(
                $"Missing required env var '{BOOTSTRAP_KEY}'. " +
                $"Example: set {BOOTSTRAP_KEY}=https://<name>.azconfig.io"
            );
        }
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(endpoint), new DefaultAzureCredential()).Select(KeyFilter.Any, LabelFilter.Null);
        });
        builder.Services.AddAzureAppConfiguration();
        NonSensitiveConstantRepository = new FamilyConfiguration(builder.Configuration);
        return builder;
    }

    public static IHostApplicationBuilder AddFamilyVault(this IHostApplicationBuilder builder)
    {
        if (NonSensitiveConstantRepository is null)
        {
            throw new InvalidOperationException("Family Configuration Resource Instance Not Found!!!");
        }
        FamilyVaultConfig? vaultConfig = NonSensitiveConstantRepository.BindSection<FamilyVaultConfig>("FamilyVault") ?? throw new InvalidOperationException("Family Vault Config is not found!!!");
        SensitiveConstantRepository = new FamilyVault(vaultConfig);
        return builder;
    }

    public static IHostApplicationBuilder AddConstantStoreService(this IHostApplicationBuilder builder)
    {
        if (NonSensitiveConstantRepository is null)
        {
            throw new InvalidOperationException("Family Configuration Resource Instance Not Found!!!");
        }
        else if (SensitiveConstantRepository is null)
        {
            throw new InvalidOperationException("Family Vault Resource Instance Not Found!!!");
        }
        Service = new(NonSensitiveConstantRepository, SensitiveConstantRepository);
        return builder;
    }

    public static IHostApplicationBuilder AddFamilyInsights(this IHostApplicationBuilder builder, ApplicationHostType hostType)
    {
        if (Service is null)
        {
            throw new InvalidOperationException("Constant Store Instance Not Initialized!!!");
        }
        FamilyInsightsConfig insightsConfig = Service.GetFamilyInsightsConfig();
        switch (hostType)
        {
            case ApplicationHostType.API:
                builder.Services.AddOpenTelemetry()
                    .UseAzureMonitor(options =>
                    {
                        options.ConnectionString = insightsConfig.ConnectionString;
                    });
                break;
            case ApplicationHostType.Console:
                builder.Logging.AddOpenTelemetry(logging =>
                {
                    logging.AddAzureMonitorLogExporter(options =>
                    {
                        options.ConnectionString = insightsConfig.ConnectionString;
                    });
                });
                break;
            case ApplicationHostType.Functions:
                builder.Services.AddOpenTelemetry()
                    .UseAzureMonitor(options =>
                    {
                        options.ConnectionString = insightsConfig.ConnectionString;
                    });
                break;
            case ApplicationHostType.Test:
                builder.Logging.AddOpenTelemetry(logging =>
                {
                    logging.AddAzureMonitorLogExporter(options =>
                    {
                        options.ConnectionString = insightsConfig.ConnectionString;
                    });
                });
                break;
        }
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        return builder;
    }
}
