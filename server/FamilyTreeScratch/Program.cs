using FamilyTreeLibrary;
using FamilyTreeLibrary.Data.Files;
using FamilyTreeLibrary.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using FamilyTreeLibrary.Infrastructure.Resource;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddFamilyTreeConfiguration();
        services.AddFamilyTreeVault();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.AddFamilyTreeLogger();
    }).Build();
