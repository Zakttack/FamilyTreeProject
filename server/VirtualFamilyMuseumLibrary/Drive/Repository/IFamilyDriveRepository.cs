using VirtualFamilyMuseumLibrary.Drive.Models;

namespace VirtualFamilyMuseumLibrary.Drive.Repository
{
    public interface IFamilyDriveRepository
    {
        public Task<FamilyBlobResource?> DeleteAsync(string blobName);
        public Task<FamilyBlobResource?> GetAsync(string blobName);
        public Task<FamilyBlobResource?> SaveAsync(string blobName, Stream content, FamilyContentTypes contentType);
    }
}