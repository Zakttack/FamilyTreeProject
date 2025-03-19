using FamilyTreeLibrary;
using FamilyTreeLibrary.Data.Files;
using FamilyTreeLibrary.Infrastructure;
using FamilyTreeLibrary.Infrastructure.Resource;
using FamilyTreeLibrary.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
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
    using FileStream templateStream = templateGenerator.WriteTemplate();
    FamilyTreeStaticStorage staticStorage = new(host.Services.GetRequiredService<FamilyTreeConfiguration>(), host.Services.GetRequiredService<FamilyTreeVault>());
    string blobUri = staticStorage.UploadTemplate(templateStream);
    Console.WriteLine(blobUri);
}
catch (Exception ex)
{
    logger.LogCritical(ex, "{name}: {message}\n{stackTrace}", ex.GetType().Name, ex.Message, ex.StackTrace);
}
finally
{
    host.Dispose();
}
