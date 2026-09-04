using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.Drive;
using VirtualFamilyMuseumLibrary.Drive.Domain;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Drive.Repository;
using VirtualFamilyMuseumLibrary.Models;

namespace VirtualFamilyMuseumLibraryTest.Drive.Domain
{
    // Integration tests against the real "family6f26m763wyjwkdrive" Azure Storage account,
    // exercising TemplateWriter.WriteLinesAsync end to end (WrapTemplateLine -> PackPages ->
    // iText PDF drawing -> FamilyDrive upload) rather than mocking any piece of that pipeline,
    // mirroring TemplateReaderTest/FamilyDriveTest. Every blob this suite creates is scratch data
    // named with a unique per-test family name and deleted in a finally block; nothing here reads
    // or modifies the seeded Kessler/Thornwood/Vantongeren fixtures. Requires an Azure identity
    // (e.g. `az login`) with Storage Blob Data Owner/Contributor access on the templates container.
    public class TemplateWriterTest
    {
        private IHost host = null!;
        private TemplateWriter writer = null!;
        private TemplateReader reader = null!;
        private IFamilyDriveRepository drive = null!;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Environment.SetEnvironmentVariable("FamilyConfigurationTest__Endpoint", ExtensionsTest.FAMILY_CONFIGURATION_TEST_ENDPOINT);
            Environment.SetEnvironmentVariable("FamilyVaultTest__Endpoint", ExtensionsTest.FAMILY_VAULT_TEST_ENDPOINT);

            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.Configuration.AddConstantStorePipeline(ExecutionTypes.Test);
            builder.AddFamilyInsights(ExecutionTypes.Test);
            builder.AddFamilyDrive();
            host = builder.Build();
            writer = host.Services.GetRequiredService<TemplateWriter>();
            reader = host.Services.GetRequiredService<TemplateReader>();
            drive = host.Services.GetRequiredService<IFamilyDriveRepository>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            host.Dispose();
        }

        // =====================================================================
        // WriteLinesAsync — empty template
        // =====================================================================

        [Test]
        public void WriteLinesAsyncShouldThrowArgumentExceptionForEmptyTemplate()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await writer.WriteLinesAsync(NewScratchFamilyName(), []));
        }

        // =====================================================================
        // WriteLinesAsync — upload result / blob metadata
        // =====================================================================

        [Test]
        public async Task WriteLinesAsyncShouldReturnBlobResourceMatchingTheUploadedTemplate()
        {
            string familyName = NewScratchFamilyName();
            try
            {
                FamilyBlobResource result = await writer.WriteLinesAsync(familyName, SmallTemplate());

                Assert.That(result, Is.Not.Null);
                Assert.That(result.ContentType, Is.EqualTo(FamilyContentTypes.Application_PDF));
                Assert.That(result.BlobName, Does.StartWith("templates/"));
                Assert.That(result.BlobName, Does.EndWith($"/{familyName}.pdf"));
                Assert.That(Uri.TryCreate(result.BlobUrl, UriKind.Absolute, out _), Is.True);
            }
            finally
            {
                await drive.DeleteAsync($"templates/{TodaysDatePath()}/{familyName}.pdf");
            }
        }

        [Test]
        public async Task WriteLinesAsyncShouldUploadAWellFormedPdf()
        {
            string familyName = NewScratchFamilyName();
            try
            {
                FamilyBlobResource result = await writer.WriteLinesAsync(familyName, SmallTemplate());

                FamilyBlobResource? downloaded = await drive.GetAsync(result.BlobName);
                Assert.That(downloaded, Is.Not.Null);

                using MemoryStream buffer = new();
                await downloaded!.Content.CopyToAsync(buffer);
                byte[] header = buffer.ToArray()[..5];
                Assert.That(System.Text.Encoding.ASCII.GetString(header), Is.EqualTo("%PDF-"));
            }
            finally
            {
                await drive.DeleteAsync($"templates/{TodaysDatePath()}/{familyName}.pdf");
            }
        }

        // =====================================================================
        // WriteLinesAsync -> TemplateReader.ReadTemplateAsync — round trip
        // =====================================================================

        [Test]
        public async Task WriteLinesAsyncShouldProduceAPdfThatReadsBackWithMatchingTemplateLines()
        {
            string familyName = NewScratchFamilyName();
            TemplateLine[] lines = SmallTemplate();
            try
            {
                FamilyBlobResource result = await writer.WriteLinesAsync(familyName, lines);

                IEnumerable<TemplateLine> roundTripped = await reader.ReadTemplateAsync(result.BlobName);
                Assert.That(roundTripped.ToArray(), Is.EqualTo(lines));
            }
            finally
            {
                await drive.DeleteAsync($"templates/{TodaysDatePath()}/{familyName}.pdf");
            }
        }

        [Test]
        public async Task WriteLinesAsyncShouldSpanMultiplePagesForALargeTemplateAndRoundTripCorrectly()
        {
            // 60 short, single-physical-line entries: 49 fit on page 1, the remaining 11 spill
            // onto page 2. This exercises PackPages' page-splitting, the per-page PdfCanvas
            // drawing loop, and GetPdfPageLines' per-page reconstruction together for real,
            // rather than any one of them in isolation.
            string familyName = NewScratchFamilyName();
            TemplateLine[] lines = LargeTemplate(60);
            try
            {
                FamilyBlobResource result = await writer.WriteLinesAsync(familyName, lines);

                IEnumerable<TemplateLine> roundTripped = await reader.ReadTemplateAsync(result.BlobName);
                Assert.That(roundTripped.ToArray(), Is.EqualTo(lines));
            }
            finally
            {
                await drive.DeleteAsync($"templates/{TodaysDatePath()}/{familyName}.pdf");
            }
        }

        // =====================================================================
        // WriteLinesAsync — overwrite
        // =====================================================================

        [Test]
        public async Task WriteLinesAsyncShouldOverwriteAnExistingTemplateForTheSameFamilyNameAndDay()
        {
            string familyName = NewScratchFamilyName();
            try
            {
                FamilyBlobResource first = await writer.WriteLinesAsync(familyName, SmallTemplate());
                TemplateLine[] secondLines = LargeTemplate(5);
                FamilyBlobResource second = await writer.WriteLinesAsync(familyName, secondLines);

                Assert.That(second.BlobName, Is.EqualTo(first.BlobName));

                IEnumerable<TemplateLine> roundTripped = await reader.ReadTemplateAsync(second.BlobName);
                Assert.That(roundTripped.ToArray(), Is.EqualTo(secondLines));
            }
            finally
            {
                await drive.DeleteAsync($"templates/{TodaysDatePath()}/{familyName}.pdf");
            }
        }

        // =====================================================================
        // Fixtures
        // =====================================================================

        private static string NewScratchFamilyName()
        {
            return $"IntegrationTest{Guid.NewGuid():N}";
        }

        // Mirrors TemplateWriter's own date-to-blob-path logic so tests can predict the blob
        // name it derives, without hardcoding "today" or duplicating the Month-enum switch.
        private static string TodaysDatePath()
        {
            DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);
            string month = date.Month switch
            {
                1 => "Jan", 2 => "Feb", 3 => "Mar", 4 => "Apr", 5 => "May", 6 => "Jun",
                7 => "Jul", 8 => "Aug", 9 => "Sep", 10 => "Oct", 11 => "Nov", 12 => "Dec",
                _ => throw new InvalidOperationException("Unexpected Month.")
            };
            return $"{date.Year}/{month}/{date.Day}";
        }

        private static TemplateLine[] SmallTemplate()
        {
            return
            [
                new()
                {
                    Coordinate = new([1]),
                    MemberBirthName = "Todd Solo",
                    MemberBirthDate = new("1975"),
                },
                new()
                {
                    Coordinate = new([1,1]),
                    MemberBirthName = "Kit Dale Kessler",
                },
                new()
                {
                    Coordinate = new([2]),
                    MemberBirthName = "Colby Bryan Kessler",
                    MemberBirthDate = new("1888", Month.Feb, 20),
                    MemberDeceasedDate = new("1942", Month.Apr),
                    InLawBirthName = "Gladys Samantha Tufty",
                    InLawBirthDate = new("1909", Month.Feb, 3),
                    FamilyDynamicStartDate = new("1941", Month.Apr, 25),
                },
            ];
        }

        private static TemplateLine[] LargeTemplate(int count)
        {
            return [.. Enumerable.Range(1, count).Select(i => new TemplateLine
            {
                Coordinate = new([i]),
                MemberBirthName = $"Test Person {i:D4}",
                MemberBirthDate = new((1900 + i).ToString()),
            })];
        }
    }
}
