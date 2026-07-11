using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace VirtualFamilyMuseumLibrary
{
    public static class Extensions
    {
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
    }
}