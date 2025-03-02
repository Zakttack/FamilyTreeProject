using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FamilyTreeLibrary.Infrastructure.Resource;
namespace FamilyTreeLibrary.Data.Files
{
    public class FamilyTreeStaticStorage(FamilyTreeConfiguration configuration, FamilyTreeVault vault)
    {
        private readonly string connectionString = vault["StorageAccountConnectionString"].AsString;
        private readonly FamilyTreeConfiguration configuration = configuration;
        
        public static void ArchiveImage(string blobUri)
        {
            BlobClient client = new(new(blobUri));
            client.SetAccessTier(AccessTier.Archive);
        }
        public static MemoryStream GetStream(string blobUri)
        {
            BlobClient client = new(new(blobUri));
            MemoryStream stream = new();
            client.DownloadTo(stream);
            stream.Position = 0;
            return stream;
        }

        public string UploadImage(FileStream imageStream)
        {
            BlobServiceClient blobService = new(connectionString);
            BlobContainerClient imageContainer = blobService.GetBlobContainerClient(configuration["Storage:Containers:Images"]);
            BlobClient image = imageContainer.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(imageStream.Name));
            image.Upload(imageStream);
            return image.Uri.ToString();
        }

        public string UploadTemplate(FileStream templateStream)
        {
            BlobServiceClient blobService = new(connectionString);
            BlobContainerClient templateContainer = blobService.GetBlobContainerClient(configuration["Storage:Containers:Templates"]);
            BlobClient template = templateContainer.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(templateStream.Name));
            template.Upload(templateStream);
            return template.Uri.ToString();
        }

    }
}