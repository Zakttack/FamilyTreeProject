using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Drive.Repository;

namespace VirtualFamilyMuseumLibraryTest.Drive.Repository
{
    // Integration tests against the real "family6f26m763wyjwkdrive" Azure Storage account.
    // FamilyDrive:BlobServiceEndpoint/ImageContainerName/TemplateContainerName already live in
    // the Test Azure App Configuration store, so configuration is pulled from there (same as
    // ExtensionsTest) via the generic host, mirroring how an ACA-hosted API bootstraps at
    // startup (logging/AddFamilyInsights is skipped since it's irrelevant to this repository).
    // FamilyDrive authenticates via DefaultAzureCredential, so running these requires an Azure
    // identity (e.g. `az login`) with Storage Blob Data Owner/Contributor access.
    public class FamilyDriveTest
    {
        // Seeded test data: 3 real PDF blobs living in the templates container. These are
        // read-only fixtures for this suite and must never be modified or deleted here.
        private const string KESSLER_TEMPLATE_BLOB_NAME = "templates/2026/Aug/14/Kessler#266518394";
        private const string KESSLER_TEMPLATE_BLOB_URL = "https://family6f26m763wyjwkdrive.blob.core.windows.net/templates/2026/Aug/14/Kessler%23266518394";
        private const long KESSLER_TEMPLATE_BLOB_LENGTH = 122576;

        private const string THORNWOOD_TEMPLATE_BLOB_NAME = "templates/2026/Aug/14/Thornwood#1030668903";
        private const string THORNWOOD_TEMPLATE_BLOB_URL = "https://family6f26m763wyjwkdrive.blob.core.windows.net/templates/2026/Aug/14/Thornwood%231030668903";
        private const long THORNWOOD_TEMPLATE_BLOB_LENGTH = 126008;

        private const string VANTONGEREN_TEMPLATE_BLOB_NAME = "templates/2026/Aug/14/Vantongeren#1545226455";
        private const string VANTONGEREN_TEMPLATE_BLOB_URL = "https://family6f26m763wyjwkdrive.blob.core.windows.net/templates/2026/Aug/14/Vantongeren%231545226455";
        private const long VANTONGEREN_TEMPLATE_BLOB_LENGTH = 121597;

        private IHost host = null!;
        private IFamilyDriveRepository drive = null!;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Environment.SetEnvironmentVariable("FamilyConfigurationTest__Endpoint", ExtensionsTest.FAMILY_CONFIGURATION_TEST_ENDPOINT);
            Environment.SetEnvironmentVariable("FamilyVaultTest__Endpoint", ExtensionsTest.FAMILY_VAULT_TEST_ENDPOINT);

            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.Configuration.AddConstantStorePipeline(ExecutionTypes.Test);
            builder.Services.AddSingleton<IFamilyDriveRepository, FamilyDrive>();

            host = builder.Build();
            drive = host.Services.GetRequiredService<IFamilyDriveRepository>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            host.Dispose();
        }

        // =====================================================================
        // GetAsync — seeded template blobs (read-only)
        // =====================================================================

        [Test]
        public async Task GetAsyncShouldReturnKesslerTemplateBlobWithExpectedMetadata()
        {
            FamilyBlobResource? result = await drive.GetAsync(KESSLER_TEMPLATE_BLOB_NAME);
            await AssertMatchesSeededTemplate(result, KESSLER_TEMPLATE_BLOB_NAME, KESSLER_TEMPLATE_BLOB_URL, KESSLER_TEMPLATE_BLOB_LENGTH);
        }

        [Test]
        public async Task GetAsyncShouldReturnThornwoodTemplateBlobWithExpectedMetadata()
        {
            FamilyBlobResource? result = await drive.GetAsync(THORNWOOD_TEMPLATE_BLOB_NAME);
            await AssertMatchesSeededTemplate(result, THORNWOOD_TEMPLATE_BLOB_NAME, THORNWOOD_TEMPLATE_BLOB_URL, THORNWOOD_TEMPLATE_BLOB_LENGTH);
        }

        [Test]
        public async Task GetAsyncShouldReturnVantongerenTemplateBlobWithExpectedMetadata()
        {
            FamilyBlobResource? result = await drive.GetAsync(VANTONGEREN_TEMPLATE_BLOB_NAME);
            await AssertMatchesSeededTemplate(result, VANTONGEREN_TEMPLATE_BLOB_NAME, VANTONGEREN_TEMPLATE_BLOB_URL, VANTONGEREN_TEMPLATE_BLOB_LENGTH);
        }

        private static async Task AssertMatchesSeededTemplate(FamilyBlobResource? result, string expectedBlobName, string expectedBlobUrl, long expectedLength)
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.BlobName, Is.EqualTo(expectedBlobName));
            Assert.That(result.BlobUrl, Is.EqualTo(expectedBlobUrl));
            Assert.That(result.ContentType, Is.EqualTo(FamilyContentTypes.Application_PDF));

            using MemoryStream buffer = new();
            await result.Content.CopyToAsync(buffer);
            Assert.That(buffer.Length, Is.EqualTo(expectedLength));
        }

        // =====================================================================
        // GetAsync — not found
        // =====================================================================

        [Test]
        public async Task GetAsyncShouldReturnNullForNonExistentTemplateBlob()
        {
            FamilyBlobResource? result = await drive.GetAsync("templates/2026/Aug/14/DoesNotExist.pdf");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAsyncShouldReturnNullForNonExistentImageBlob()
        {
            FamilyBlobResource? result = await drive.GetAsync("images/does-not-exist.jpg");
            Assert.That(result, Is.Null);
        }

        // =====================================================================
        // GetAsync / DeleteAsync / SaveAsync — unsupported container
        // =====================================================================

        [Test]
        public void GetAsyncShouldThrowNotSupportedExceptionForUnknownContainer()
        {
            Assert.ThrowsAsync<NotSupportedException>(async () => await drive.GetAsync("documents/file.txt"));
        }

        [Test]
        public void DeleteAsyncShouldThrowNotSupportedExceptionForUnknownContainer()
        {
            Assert.ThrowsAsync<NotSupportedException>(async () => await drive.DeleteAsync("documents/file.txt"));
        }

        [Test]
        public void SaveAsyncShouldThrowNotSupportedExceptionForUnknownContainer()
        {
            using MemoryStream content = new([1, 2, 3]);
            Assert.ThrowsAsync<NotSupportedException>(async () => await drive.SaveAsync("documents/file.txt", content, FamilyContentTypes.Application_PDF));
        }

        // =====================================================================
        // SaveAsync / GetAsync / DeleteAsync — round trip against a scratch blob
        // =====================================================================

        [Test]
        public async Task SaveAsyncShouldUploadNewImageBlobAndReturnMatchingResource()
        {
            string blobName = NewScratchImageBlobName();
            try
            {
                using MemoryStream upload = new([0xFF, 0xD8, 0xFF, 0xE0]);
                FamilyBlobResource? result = await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);

                Assert.That(result, Is.Not.Null);
                Assert.That(result!.BlobName, Is.EqualTo(blobName));
                Assert.That(result.ContentType, Is.EqualTo(FamilyContentTypes.Image_JPEG));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        [Test]
        public async Task SaveAsyncShouldPersistBlobRetrievableViaGetAsync()
        {
            string blobName = NewScratchImageBlobName();
            byte[] content = [0xFF, 0xD8, 0xFF, 0xE0, 0x01, 0x02];
            try
            {
                using (MemoryStream upload = new(content))
                {
                    await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);
                }

                FamilyBlobResource? result = await drive.GetAsync(blobName);
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.ContentType, Is.EqualTo(FamilyContentTypes.Image_JPEG));

                using MemoryStream downloaded = new();
                await result.Content.CopyToAsync(downloaded);
                Assert.That(downloaded.ToArray(), Is.EqualTo(content));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        [Test]
        public async Task SaveAsyncShouldOverwriteExistingBlobContent()
        {
            string blobName = NewScratchImageBlobName();
            try
            {
                using (MemoryStream first = new([1, 2, 3]))
                {
                    await drive.SaveAsync(blobName, first, FamilyContentTypes.Image_JPEG);
                }

                byte[] secondContent = [9, 8, 7, 6, 5];
                using (MemoryStream second = new(secondContent))
                {
                    await drive.SaveAsync(blobName, second, FamilyContentTypes.Image_JPEG);
                }

                FamilyBlobResource? result = await drive.GetAsync(blobName);
                Assert.That(result, Is.Not.Null);

                using MemoryStream downloaded = new();
                await result!.Content.CopyToAsync(downloaded);
                Assert.That(downloaded.ToArray(), Is.EqualTo(secondContent));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        [Test]
        public async Task DeleteAsyncShouldRemoveExistingBlobAndReturnItsFinalContent()
        {
            string blobName = NewScratchImageBlobName();
            byte[] content = [10, 20, 30];
            using (MemoryStream upload = new(content))
            {
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);
            }

            FamilyBlobResource? deleted = await drive.DeleteAsync(blobName);
            Assert.That(deleted, Is.Not.Null);
            Assert.That(deleted!.BlobName, Is.EqualTo(blobName));

            using MemoryStream downloaded = new();
            await deleted.Content.CopyToAsync(downloaded);
            Assert.That(downloaded.ToArray(), Is.EqualTo(content));

            FamilyBlobResource? afterDelete = await drive.GetAsync(blobName);
            Assert.That(afterDelete, Is.Null);
        }

        [Test]
        public async Task DeleteAsyncShouldReturnNullForNonExistentBlob()
        {
            string blobName = NewScratchImageBlobName();
            FamilyBlobResource? result = await drive.DeleteAsync(blobName);
            Assert.That(result, Is.Null);
        }

        private static string NewScratchImageBlobName()
        {
            return $"images/integration-test-{Guid.NewGuid()}.jpg";
        }
    }
}
