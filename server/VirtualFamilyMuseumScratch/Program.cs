// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.Configuration;
using VirtualFamilyMuseumLibrary.Configuration.Models;
using VirtualFamilyMuseumLibrary.Configuration.Repositories;

HostApplicationBuilder? builder = Host.CreateApplicationBuilder(args);
Environment.SetEnvironmentVariable("FAMILY_CONFIGURATION_URI", "https://appconfig-virtual-family-museum.azconfig.io");
builder.AddFamilyConfiguration();
builder.AddFamilyVault();
IHost host = builder.Build();
INonSensitiveConstantRepository nonSensitiveStore = host.Services.GetRequiredService<INonSensitiveConstantRepository>();
ISensitiveConstantRepository sensitiveStore = host.Services.GetRequiredService<ISensitiveConstantRepository>();
ConfigurationService service = new(nonSensitiveStore, sensitiveStore);
Console.WriteLine(await service.GetFamilyInsightsConfig());
