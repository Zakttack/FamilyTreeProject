using Microsoft.Extensions.Logging;
using System.Collections;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Drive.Repository;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace VirtualFamilyMuseumLibrary.Drive.Domain
{
    public class TemplateReader(IFamilyDriveRepository repositoryIn, ILogger<TemplateReader> loggerIn)
    {
        private readonly IFamilyDriveRepository repository = repositoryIn;
        private readonly ILogger<TemplateReader> logger = loggerIn;

        public async Task<IEnumerable<TemplateLine>> ReadTemplateAsync(string blobName)
        {
            logger.LogInformation("Retrieving {BlobName} from the family drive.", blobName);
            FamilyBlobResource? blobResource = await repository.GetAsync(blobName);
            if (blobResource is null)
            {
                // Log the failure at Warning, with BlobName as a structured field, right where it's
                // detected — not just inside the exception message. This is the one line an App
                // Insights query for "why did reads fail this week" actually needs; without it the
                // only trace of a not-found blob is the exception text bubbling up through whatever
                // caught it (or nothing, if the caller only logs unhandled exceptions).
                logger.LogWarning("{BlobName} isn't found within the templates container of the family drive.", blobName);
                throw new InvalidOperationException($"{blobName} isn't found within the templates container of the family drive.");
            }
            if (blobResource.ContentType != FamilyContentTypes.Application_PDF)
            {
                // Same reasoning as above, for the other thrown path. Including ContentType here
                // (and not just in the exception message) means a query can group "wrong content
                // type" failures by what they actually were, e.g. "how many were JPEGs".
                logger.LogWarning("{BlobName} isn't a template ({ContentType}).", blobName, blobResource.ContentType);
                throw new InvalidOperationException($"{blobName} isn't a template.");
            }
            logger.LogInformation("Opening {BlobName} for reading.", blobResource.BlobName);
            return new TemplateLineEnumerable(blobResource, logger);
        }

        private sealed class TemplateLineEnumerable(FamilyBlobResource blobResource, ILogger<TemplateReader> logger) : IEnumerable<TemplateLine>
        {
            public IEnumerator<TemplateLine> GetEnumerator()
            {
                return new TemplateLineEnumerator(blobResource, logger);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new TemplateLineEnumerator(blobResource, logger);
            }
        }

        private sealed class TemplateLineEnumerator : IEnumerator<TemplateLine>
        {
            private readonly ILogger<TemplateReader> logger;
            private readonly FamilyBlobResource blobResource;
            private readonly PdfReader reader;
            private readonly PdfDocument document;
            private TemplateLine? current;
            private int pageNumber;
            private int lineCount;
            private readonly Queue<string> pdfLines;

            public TemplateLineEnumerator(FamilyBlobResource blobResource, ILogger<TemplateReader> logger)
            {
                this.logger = logger;
                this.blobResource = blobResource;
                if (blobResource.Content.CanSeek)
                {
                    blobResource.Content.Position = 0;
                }
                reader = new(blobResource.Content);
                document = new(reader);
                pageNumber = 0;
                lineCount = 0;
                pdfLines = new();
            }

            public TemplateLine Current
            {
                get
                {
                    if (current is null)
                    {
                        throw new InvalidOperationException("The enumerator is not positioned on a valid template line.");
                    }
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (current is null)
                    {
                        throw new InvalidOperationException("The enumerator is not positioned on a valid template line.");
                    }
                    return current;
                }
            }

            public void Dispose()
            {
                // One summary line per read, at Information, replacing what used to be a per-line
                // "checking..." statement (see MoveNext below). This single line answers the
                // question actually asked when diagnosing a read: "how far did it get, and across
                // how many pages" — and it answers it whether Dispose was reached via normal
                // completion or an exception unwinding through the compiler-generated foreach/using.
                // It's exactly one Information-level entry per ReadTemplateAsync call, so App
                // Insights ingestion cost scales with reads, not with template lines or pages —
                // the opposite of the per-MoveNext() logging it replaces.
                logger.LogInformation("Closed {BlobName} after producing {LineCount} template line(s) across {PageCount} page(s).", blobResource.BlobName, lineCount, pageNumber);
                document.Close();
                reader.Close();
                current = null;
                pageNumber = 0;
                while (pdfLines.TryDequeue(out _)) { }
            }

            public bool MoveNext()
            {
                if (pdfLines.TryDequeue(out string? line) && line is not null)
                {
                    current = line.AsTemplateLine();
                    lineCount++;

                    // Kept at Debug, but now the only place a line's content gets logged — the
                    // original code logged the same text twice (once here, once again while
                    // normalizing the page below). Debug is off by default in production, so
                    // logging full member names/dates here only reaches a sink when a developer
                    // has deliberately raised the level to investigate a specific read; that's the
                    // right gate for PII this granular, rather than letting it flow through
                    // Information on every run regardless of whether anyone's debugging.
                    logger.LogDebug("Current template line: {TemplateLine}", current);
                    return true;
                }
                else if (pageNumber >= document.GetNumberOfPages())
                {
                    return false;
                }
                pageNumber++;
                PdfPage page = document.GetPage(pageNumber);
                string pageText = PdfTextExtractor.GetTextFromPage(page);
                Queue<string> normalizedLines = DriveExtensions.GetPdfPageLines(pageText.Split('\n'));
                // One Debug line per page (not per line), carrying BlobName so a page-level
                // problem can be traced back to its document — the original per-page logging
                // never included it — plus just enough shape (character/line counts) to spot a
                // bad extraction without re-logging PII.
                logger.LogDebug("Extracted {CharacterCount} characters from page {PageNumber} of {BlobName}, normalized to {NormalizedLineCount} line(s).",
                    pageText.Length, pageNumber, blobResource.BlobName, normalizedLines.Count);
                while (normalizedLines.TryDequeue(out string? normalizedLine) && normalizedLine is not null)
                {
                    pdfLines.Enqueue(normalizedLine);
                }

                return MoveNext();
            }

            public void Reset()
            {
                throw new NotSupportedException("Reset isn't supported.");
            }
        }
    }
}
