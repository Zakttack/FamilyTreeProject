using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Drive.Repository;
using VirtualFamilyMuseumLibrary.Models;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace VirtualFamilyMuseumLibrary.Drive.Domain
{
    public class TemplateWriter(ILogger<TemplateWriter> loggerIn, IConfiguration configurationIn, IFamilyDriveRepository repositoryIn)
    {
        private readonly ILogger<TemplateWriter> logger = loggerIn;
        private readonly IFamilyDriveRepository repository = repositoryIn;
        private readonly IConfiguration configuration = configurationIn;

        public async Task<FamilyBlobResource> WriteLinesAsync(string inheritedFamilyName, IEnumerable<TemplateLine> templateContent)
        {
            logger.LogInformation("Writing a template for {InheritedFamilyName} to the family drive.", inheritedFamilyName);
            if (!templateContent.Any())
            {
                logger.LogError("Template for {InheritedFamilyName} is empty; nothing to write.", inheritedFamilyName);
                throw new InvalidOperationException("Template can't be empty.");
            }
            IList<IList<string>> physicalLineGroups = [];
            foreach (TemplateLine line in templateContent)
            {
                IList<string> wrapped = DriveExtensions.WrapTemplateLine(line.ToString());
                logger.LogDebug("Wrapped template line {TemplateLine} into {PhysicalLineCount} physical line(s).", line, wrapped.Count);
                physicalLineGroups.Add(wrapped);
            }
            IEnumerable<string[]> pages = DriveExtensions.PackPages(physicalLineGroups);
            DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);
            Month normalizedMonth = date.Month switch
            {
                1 => Month.Jan,
                2 => Month.Feb,
                3 => Month.Mar,
                4 => Month.Apr,
                5 => Month.May,
                6 => Month.Jun,
                7 => Month.Jul,
                8 => Month.Aug,
                9 => Month.Sep,
                10 => Month.Oct,
                11 => Month.Nov,
                12 => Month.Dec,
                _ => throw new InvalidOperationException($"Unexpected Month value: {date.Month}")
            };
            FamilyDate normalizedDate = new(date.Year.ToString(), normalizedMonth, date.Day);
            string blobName = $"{configuration["FamilyDrive:TemplateContainerName"]}/{normalizedDate.Year}/{normalizedDate.Month}/{normalizedDate.Day}/{inheritedFamilyName}.pdf";
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            const float MARGIN_LEFT = 50f;
            const float TOP_MARGIN = 750f;
            const float LINE_HEIGHT = 14f;
            using Stream content = new MemoryStream();
            using PdfWriter writer = new(content);
            writer.SetCloseStream(false);
            using PdfDocument pdf = new(writer);
            int pageNumber = 0;
            foreach (string[] page in pages)
            {
                pageNumber++;
                PdfPage currentPage = pdf.AddNewPage(PageSize.LETTER);
                PdfCanvas canvas = new(currentPage);
                canvas.SetFontAndSize(font, 11f);
                float y = TOP_MARGIN;
                int lineCount = 0;
                foreach (string? line in page)
                {
                    if (line is null)
                    {
                        break;
                    }
                    canvas.BeginText()
                        .MoveText(MARGIN_LEFT, y)
                        .ShowText(line)
                        .EndText();
                    y -= LINE_HEIGHT;
                    lineCount++;
                }
                logger.LogDebug("Drew page {PageNumber} of {BlobName} with {LineCount} line(s).", pageNumber, blobName, lineCount);
            }
            pdf.Close();
            FamilyBlobResource? resource = await repository.SaveAsync(blobName, content, FamilyContentTypes.Application_PDF);
            if (resource is null)
            {
                logger.LogError("Unable to upload {BlobName} to the templates container.", blobName);
                throw new IOException("Unable to upload to the templates container.");
            }
            logger.LogInformation("Wrote template for {InheritedFamilyName} to {BlobName} across {PageCount} page(s).", inheritedFamilyName, blobName, pageNumber);
            return resource;
        }
    }
}