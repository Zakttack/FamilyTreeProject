using Microsoft.Extensions.Logging;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Drive.Repository;

namespace VirtualFamilyMuseumLibrary.Drive.Domain
{
    public class FamilyDriveService(ILogger<FamilyDriveService> loggerIn, IFamilyDriveRepository repositoryIn)
    {
        private readonly ILogger<FamilyDriveService> logger = loggerIn;
        private readonly IFamilyDriveRepository repository = repositoryIn;

        public async Task<FamilyDriveResult<FamilyBlobResource>> DownloadImageAsync(string blobName)
        {
            logger.LogInformation("Downloading {BlobName} from the family drive.", blobName);

            if (blobName is null)
            {
                // GetContainer below assumes a non-null string (it calls blobName.Split('/')
                // directly), so a null here would otherwise surface as a raw NullReferenceException
                // instead of an intentional, well-described exception.
                ArgumentNullException ex = new(nameof(blobName));
                logger.LogError(ex, "Blob name can't be null.");
                throw ex;
            }

            // GetContainer throws NotSupportedException for any name that isn't images/templates
            // prefixed (including "" — see DriveExtensionsTest). Folded into the same
            // InvalidOperationException below so callers see one exception type — and one
            // description — for "this isn't a valid images blob name," regardless of whether the
            // name was recognized-but-wrong-container or unrecognized outright.
            bool isImageBlobName;
            try
            {
                isImageBlobName = DriveExtensions.GetContainer(blobName) == FamilyDriveContainers.Images;
            }
            catch (NotSupportedException)
            {
                isImageBlobName = false;
            }
            if (!isImageBlobName)
            {
                InvalidOperationException ex = new($"{blobName} doesn't belong to the images container.");
                logger.LogError(ex, "{BlobName} doesn't belong to the images container.", blobName);
                throw ex;
            }

            FamilyBlobResource? resource = await repository.GetAsync(blobName);
            if (resource is null)
            {
                FileNotFoundException ex = new($"{blobName} isn't found in the images container.", blobName);
                logger.LogError(ex, "{BlobName} isn't found in the images container.", blobName);
                throw ex;
            }

            logger.LogInformation("{BlobName} has been found.", blobName);
            return new FamilyDriveResult<FamilyBlobResource>
            {
                Status = FamilyDriveResultStatuses.Success,
                Message = $"{blobName} ({resource.ContentType.GetContentType()}) has been downloaded.",
                Payload = resource
            };
        }
    }
}