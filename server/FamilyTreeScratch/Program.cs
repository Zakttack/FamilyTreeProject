using FamilyTreeLibrary;
using FamilyTreeLibrary.Data.Files;
using FamilyTreeLibrary.Infrastructure;
using FamilyTreeLibrary.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

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
    using PdfReader reader = new(templateStream);
    using PdfDocument document = new(reader);
    for (int page = 1; page <= document.GetNumberOfPages(); page++)
    {
        string[] pageContent = PdfTextExtractor.GetTextFromPage(document.GetPage(1)).Split('\n', StringSplitOptions.None);
        foreach (string content in pageContent)
        {
            Console.WriteLine(content);
        }
    }
    document.Close();
    reader.Close();
    templateStream.Close();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "{name}: {message}\n{stackTrace}", ex.GetType().Name, ex.Message, ex.StackTrace);
}
finally
{
    host.Dispose();
}
