// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.ConstantStore;
using VirtualFamilyMuseumLibrary.ConstantStore.Models;
using VirtualFamilyMuseumLibrary.ConstantStore.Repositories;

HostApplicationBuilder? builder = Host.CreateApplicationBuilder(args);
Environment.SetEnvironmentVariable("FAMILY_CONFIGURATION_URI", "https://appconfig-virtual-family-museum.azconfig.io");
builder.AddFamilyConfiguration();
builder.AddFamilyVault();
builder.AddConstantStoreService();
builder.AddFamilyInsights();
IHost host = builder.Build();
ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("This is an information Message.");
