using FamilyTreeLibrary;
using FamilyTreeLibrary.Data.Databases;
using FamilyTreeLibrary.Data.Files;
using FamilyTreeLibrary.Infrastructure;
using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Logging;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddFamilyTreeConfiguration();
        services.AddFamilyTreeVault();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();
        logging.AddFamilyTreeLogger();
        logging.SetMinimumLevel(LogLevel.Debug);
    }).Build();
IExtendedLogger<Program> logger = host.Services.GetRequiredService<IExtendedLogger<Program>>();
try
{
    ITemplateGenerator templateGenerator = new TemplateGenerator(host.Services.GetRequiredService<IExtendedLogger<TemplateGenerator>>());
    templateGenerator.WriteTemplate();
    Console.WriteLine("You can view the pdf.");
}
catch (Exception ex)
{
    logger.LogCritical(ex, "{name}: {message}\n{stackTrace}", ex.GetType().Name, ex.Message, ex.StackTrace);
    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
}
