using FamilyTreeLibrary;
using FamilyTreeLibrary.Data.Databases;
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
    PersonCollection collection = new(host.Services.GetRequiredService<FamilyTreeConfiguration>(), host.Services.GetRequiredService<FamilyTreeVault>());
    Person p = new(new Dictionary<string,BridgeInstance>() 
    {
        ["birthName"] = new("Zak Ray Merrigan"),
        ["birthDate"] = new("25 Jul 1999"),
        ["deceasedDate"] = new()
    }, true);
    collection[p.Id] = p.Copy();
    Console.WriteLine("Done");
}
catch (Exception ex)
{
    logger.LogCritical(ex, "{name}: {message}\n{stackTrace}", ex.GetType().Name, ex.Message, ex.StackTrace);
}
