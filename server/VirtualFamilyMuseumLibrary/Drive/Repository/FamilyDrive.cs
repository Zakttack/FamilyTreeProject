using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using VirtualFamilyMuseumLibrary.Drive.Models;

namespace VirtualFamilyMuseumLibrary.Drive.Repository
{
    public class FamilyDrive : IFamilyDriveRepository
    {
        private readonly BlobContainerClient imageClient;
        private readonly BlobContainerClient templateClient;

        public FamilyDrive(IConfiguration configuration)
        {
            Uri accountUri = new(configuration["FamilyDrive:BlobServiceEndpoint"]!);
            BlobServiceClient client = new(accountUri, new DefaultAzureCredential());
            imageClient = client.GetBlobContainerClient(configuration["FamilyDrive:ImageContainerName"]!);
            templateClient = client.GetBlobContainerClient(configuration["FamilyDrive:TemplateContainerName"]!);
        }

        public async Task<FamilyBlobResource?> DeleteAsync(string blobName)
        {
            FamilyDriveContainers? containerType = DriveExtensions.GetContainer(blobName);
            BlobClient? blob = containerType switch
            {
                FamilyDriveContainers.Images => imageClient.GetBlobClient(blobName),
                FamilyDriveContainers.Templates => templateClient.GetBlobClient(blobName),
                _ => null
            };
            if (blob is null || !await blob.ExistsAsync())
            {
                return null;
            }
            Response<BlobDownloadStreamingResult> downloadResponse = await blob.DownloadStreamingAsync();
            FamilyBlobResource blobResource = new()
            {
                BlobName = blobName,
                BlobUrl = blob.Uri.ToString(),
                Content = downloadResponse.Value.Content,
                ContentType = downloadResponse.Value.Details.ContentType.GetContentType()
            };
            await blob.DeleteAsync();
            return blobResource;
        }

        public async Task<FamilyBlobResource?> GetAsync(string blobName)
        {
            FamilyDriveContainers containerType = DriveExtensions.GetContainer(blobName);
            BlobClient? blob = containerType switch
            {
                FamilyDriveContainers.Images => imageClient.GetBlobClient(blobName),
                FamilyDriveContainers.Templates => templateClient.GetBlobClient(blobName),
                _ => null
            };
            if (blob is null || !await blob.ExistsAsync())
            {
                return null;
            }
            Response<BlobDownloadStreamingResult> downloadResponse = await blob.DownloadStreamingAsync();
            return new()
            {
                BlobName = blobName,
                BlobUrl = blob.Uri.ToString(),
                Content = downloadResponse.Value.Content,
                ContentType = downloadResponse.Value.Details.ContentType.GetContentType()
            };
        }

        public async Task<FamilyBlobResource?> UploadAsync(string blobName, Stream content, FamilyContentTypes contentType)
        {
            FamilyDriveContainers containerType = DriveExtensions.GetContainer(blobName);
            BlobClient? blob = containerType switch
            {
                FamilyDriveContainers.Images => imageClient.GetBlobClient(blobName),
                FamilyDriveContainers.Templates => templateClient.GetBlobClient(blobName),
                _ => null
            };
            if (blob is null)
            {
                return null;
            }
            if (content.CanSeek)
            {
                content.Position = 0;
            }
            await blob.UploadAsync(content);
            return new()
            {
                BlobName = blobName,
                BlobUrl = blob.Uri.ToString(),
                Content = content,
                ContentType = contentType
            };
        }
    }
}