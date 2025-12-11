using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.ConstantStore;
using VirtualFamilyMuseumLibrary.ConstantStore.Models;
using VirtualFamilyMuseumLibrary.ConstantStore.Repositories;

Console.WriteLine("[DEBUG] Starting application...");

HostApplicationBuilder? builder = Host.CreateApplicationBuilder(args);
Environment.SetEnvironmentVariable("FAMILY_CONFIGURATION_URI", "https://appconfig-virtual-family-museum.azconfig.io");

Console.WriteLine("[DEBUG] Adding Family Configuration...");
builder.AddFamilyConfiguration();

Console.WriteLine("[DEBUG] Adding Family Vault...");
builder.AddFamilyVault();

Console.WriteLine("[DEBUG] Adding Constant Store Service...");
builder.AddConstantStoreService();

Console.WriteLine("[DEBUG] Adding Family Insights...");
builder.AddFamilyInsights(ApplicationHostType.Console);

// Verify connection string
FamilyInsightsConfig? config = FamilyUtils.Service?.GetFamilyInsightsConfig();
Console.WriteLine($"[DEBUG] Connection String: {config?.ConnectionString?.Substring(0, 50)}...");

Console.WriteLine("[DEBUG] Building host...");
using IHost host = builder.Build();

Console.WriteLine("[DEBUG] Getting logger...");
ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();

Console.WriteLine("[DEBUG] Logging test message...");
logger.LogInformation("This is an information Message.");
logger.LogWarning("This is a warning message for testing.");
logger.LogError("This is an error message for testing.");

Console.WriteLine("[DEBUG] Waiting 10 seconds for telemetry flush...");
await Task.Delay(10000); // Increased to 10 seconds

Console.WriteLine("[DEBUG] Stopping host...");
await host.StopAsync();

Console.WriteLine("[DEBUG] Application complete!");