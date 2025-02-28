using Azure.Identity;
using FamilyTreeLibrary.Infrastructure.Resource;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyTreeLibrary.Infrastructure
{
    public static class ConfigurationUtils
    {
        private const string API_CONFIGURATION_URI = "https://familytreeconfiguration.azconfig.io";
        private static readonly FamilyTreeConfiguration configuration = new(API_CONFIGURATION_URI);

        public static IServiceCollection AddFamilyTreeConfiguration(this IServiceCollection services)
        {
            return services.AddSingleton((provider) => 
            {
                ConfigureApplicationInsights();
                ConfigureKeyVault();
                ConfigureStorageSettings();
                return configuration;
            });
        }

        public static IConfigurationBuilder AddFamilyTreeConfiguration(this IConfigurationBuilder builder)
        {
            return builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(API_CONFIGURATION_URI), new DefaultAzureCredential()).Select("*");
            });
        }

        public static IServiceCollection AddFamilyTreeVault(this IServiceCollection services)
        {
            services.AddSingleton<FamilyTreeVault>(provider =>
            {
                FamilyTreeConfiguration configuration = provider.GetService<FamilyTreeConfiguration>()!;
                return new(configuration);
            });
            return services;
        }

        private static void ConfigureApplicationInsights()
        {
            configuration["ApplicationInsights:Name"] = "familyTreeInsights";
            configuration["ApplicationInsights:Type"] = "web";
        }

        private static void ConfigureKeyVault()
        {
            configuration["KeyVault:Uri"] = "https://familytreevault.vault.azure.net/";
        }

        private static void ConfigureStorageSettings()
        {
            configuration["Storage:Containers:Logs"] = "logs";
            configuration["Storage:Containers:Images"] = "images";
            configuration["Storage:Containers:Templates"] = "templates";
            configuration["Storage:AccountName"] = "familytreestaticstorage";
            configuration["Storage:Url"] = "https://familytreestaticstorage.blob.core.windows.net/";
            configuration["Storage:LifecyclePolicies:Logs:CoolTierDays"] = "0";
            configuration["Storage:LifecyclePolicies:Logs:ArchiveTierDays"] = "90";
            configuration["Storage:LifecyclePolicies:Logs:DeleteDays"] = "180";
            configuration["Storage:LifecyclePolicies:Templates:ArchiveTierDays"] = "90";
            configuration["Storage:LifecyclePolicies:Templates:DeleteDays"] = "180";
            configuration["Storage:LifecyclePolicies:Images:ArchiveTierDays"] = "0";
            configuration["Storage:LifecyclePolicies:Images:DeleteDays"] = "180";
        }
    }
}