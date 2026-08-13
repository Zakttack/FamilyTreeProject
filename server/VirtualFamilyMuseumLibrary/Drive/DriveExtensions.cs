using VirtualFamilyMuseumLibrary.Drive.Models;

namespace VirtualFamilyMuseumLibrary.Drive
{
    public static class DriveExtensions
    {
        public static FamilyDriveContainers GetContainer(string blobName)
        {
            string containerName = blobName.Split('/').First();
            return containerName switch
            {
                "images" => FamilyDriveContainers.Images,
                "templates" => FamilyDriveContainers.Templates,
                _ => throw new NotSupportedException($"{containerName} isn't part of the family drive.")
            };
        }

        public static string GetContentType(this FamilyContentTypes contentType)
        {
            return contentType.ToString().Replace('_', '/').ToLower();
        }

        public static FamilyContentTypes GetContentType(this string text)
        {
            return text switch
            {
                "application/pdf" => FamilyContentTypes.Application_PDF,
                "image/jpeg" => FamilyContentTypes.Image_JPEG,
                _ => throw new NotSupportedException($"{text} isn't a supported content-type.")
            };
        }
    }
}