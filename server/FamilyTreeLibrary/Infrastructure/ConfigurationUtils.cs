using System.Diagnostics;
using Azure.Identity;
using FamilyTreeLibrary.Infrastructure.Resource;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyTreeLibrary.Infrastructure
{
    public static class ConfigurationUtils
    {
        private const string API_CONFIGURATION_URI = "https://familytreeconfiguration.azconfig.io";

        public static IServiceCollection AddFamilyTreeConfiguration(this IServiceCollection services)
        {
            return services.AddSingleton((provider) => 
            {
                FamilyTreeConfiguration configuration = new(API_CONFIGURATION_URI);
                ConfigureApplicationInsights(configuration);
                ConfigureKeyVault(configuration);
                ConfigureStorageSettings(configuration);
                return configuration;
            });
        }

        // public static IServiceCollection AddFamilyTreeConfigurationLocal(this IServiceCollection services)
        // {
        //     return services.AddSingleton((provider) =>
        //     {
        //         if (!IsLoggedIn())
        //         {
        //             Login();
        //         }
        //         FamilyTreeConfiguration configuration = new(API_CONFIGURATION_URI, new AzureCliCredential());
        //         ConfigureApplicationInsights(configuration);
        //         ConfigureKeyVault(configuration);
        //         ConfigureStorageSettings(configuration);
        //         return configuration;
        //     });
        // }

        public static IConfigurationBuilder AddFamilyTreeConfiguration(this IConfigurationBuilder builder)
        {
            return builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(API_CONFIGURATION_URI), new DefaultAzureCredential()).Select("*");
            });
        }

        public static IConfigurationBuilder AddFamilyTreeConfigurationLocal(this IConfigurationBuilder builder)
        {
            return builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(API_CONFIGURATION_URI), new AzureCliCredential()).Select("*");
            });
        }

        public static IServiceCollection AddFamilyTreeVault(this IServiceCollection services)
        {
            services.AddSingleton<FamilyTreeVault>(provider =>
            {
                return new(provider.GetRequiredService<FamilyTreeConfiguration>());
            });
            return services;
        }

        private static void ConfigureApplicationInsights(FamilyTreeConfiguration configuration)
        {
            configuration["ApplicationInsights:Name"] = "familyTreeInsights";
            configuration["ApplicationInsights:Type"] = "web";
        }

        private static void ConfigureKeyVault(FamilyTreeConfiguration configuration)
        {
            configuration["KeyVault:Uri"] = "https://familytreevault.vault.azure.net/";
        }

        private static void ConfigureStorageSettings(FamilyTreeConfiguration configuration)
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

        // private static bool IsLoggedIn()
        // {
        //     try
        //     {
        //         Process process = new()
        //         {
        //             StartInfo = new ProcessStartInfo()
        //             {
        //                 FileName = "az",
        //                 Arguments = "account show",
        //                 RedirectStandardError = true,
        //                 RedirectStandardOutput = true,
        //                 CreateNoWindow = true,
        //                 UseShellExecute = false
        //             }
        //         };
        //         process.Start();
        //         string output = process.StandardOutput.ReadToEnd();
        //         string error = process.StandardError.ReadToEnd();
        //         process.WaitForExit();
        //         return process.ExitCode == 0 && !string.IsNullOrEmpty(output) && string.IsNullOrEmpty(error);
        //     }
        //     catch (Exception)
        //     {
        //         return false;
        //     }
        // }

        // private static void Login()
        // {
        //     Process process = new()
        //     {
        //         StartInfo = new()
        //         {
        //             FileName = "az",
        //             Arguments = "login",
        //             UseShellExecute = true
        //         }
        //     };
        //     process.Start();
        //     process.WaitForExit();
        // }
    }
}