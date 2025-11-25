namespace VirtualFamilyMuseumLibrary;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VirtualFamilyMuseumLibrary.Configuration.Models;
using VirtualFamilyMuseumLibrary.Configuration.Repositories;

public static class FamilyUtils
{
    private const string BOOTSTRAP_KEY = "FAMILY_CONFIGURATION_URI";

    public static IHostApplicationBuilder AddFamilyConfiguration(this IHostApplicationBuilder builder, bool allowLocalJsonFallback = true)
    {
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
        builder.Services.AddSingleton<INonSensitiveConstantRepository,FamilyConfiguration>((sp) =>
        {
            return new FamilyConfiguration(builder.Configuration);
        });
        return builder;
    }

    public static IHostApplicationBuilder AddFamilyVault(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<FamilyVaultConfig>()
            .Bind(builder.Configuration.GetSection("FamilyVault"))
            .ValidateDataAnnotations()
            .Validate(cfg => Uri.IsWellFormedUriString(cfg.Uri, UriKind.Absolute),
                "FamilyVault:Uri must be a valid absolute URI")
            .ValidateOnStart();
        builder.Services.AddSingleton<ISensitiveConstantRepository,FamilyVault>((sp) =>
        {
            FamilyVaultConfig config = sp.GetRequiredService<IOptions<FamilyVaultConfig>>().Value;
            return new FamilyVault(config);
        });
        return builder;
    }
}
