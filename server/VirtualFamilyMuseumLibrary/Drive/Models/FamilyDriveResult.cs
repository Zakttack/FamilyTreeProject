using VirtualFamilyMuseumLibrary.Models;
namespace VirtualFamilyMuseumLibrary.Drive.Models
{
    public class FamilyDriveResult<T> : DomainResult<T>
    {
        public required FamilyDriveResultStatuses Status
        {
            get;
            init;
        }
    }
}