using VirtualFamilyMuseumLibrary.Drive;
using VirtualFamilyMuseumLibrary.Drive.Models;

namespace VirtualFamilyMuseumLibraryTest.Drive
{
    public class DriveExtensionsTest
    {
        // =====================================================================
        // GetContainer(string)
        // =====================================================================

        [Test]
        public void GetContainerShouldReturnImagesForImagesBlobName()
        {
            FamilyDriveContainers result = DriveExtensions.GetContainer("images");
            Assert.That(result, Is.EqualTo(FamilyDriveContainers.Images));
        }

        [Test]
        public void GetContainerShouldReturnTemplatesForTemplatesBlobName()
        {
            FamilyDriveContainers result = DriveExtensions.GetContainer("templates");
            Assert.That(result, Is.EqualTo(FamilyDriveContainers.Templates));
        }

        [Test]
        public void GetContainerShouldReturnContainerForBlobNameWithNestedPath()
        {
            FamilyDriveContainers result = DriveExtensions.GetContainer("images/family/photo.jpg");
            Assert.That(result, Is.EqualTo(FamilyDriveContainers.Images));
        }

        [Test]
        public void GetContainerShouldReturnTemplatesForNestedTemplatesPath()
        {
            FamilyDriveContainers result = DriveExtensions.GetContainer("templates/reunion/invite.pdf");
            Assert.That(result, Is.EqualTo(FamilyDriveContainers.Templates));
        }

        [Test]
        public void GetContainerShouldThrowNotSupportedExceptionForUnknownContainer()
        {
            Assert.That(() => DriveExtensions.GetContainer("documents/file.txt"), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void GetContainerShouldThrowNotSupportedExceptionForCaseMismatchedContainerName()
        {
            Assert.That(() => DriveExtensions.GetContainer("Images/photo.jpg"), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void GetContainerShouldThrowNotSupportedExceptionForEmptyBlobName()
        {
            Assert.That(() => DriveExtensions.GetContainer(""), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void GetContainerExceptionMessageShouldIncludeOffendingContainerName()
        {
            NotSupportedException? exception = Assert.Throws<NotSupportedException>(() => DriveExtensions.GetContainer("documents/file.txt"));
            Assert.That(exception!.Message, Does.Contain("documents"));
        }

        // =====================================================================
        // GetContentType(FamilyContentTypes) — enum to MIME string
        // =====================================================================

        [Test]
        public void GetContentTypeShouldReturnApplicationPdfForApplicationPdfEnum()
        {
            string result = FamilyContentTypes.Application_PDF.GetContentType();
            Assert.That(result, Is.EqualTo("application/pdf"));
        }

        [Test]
        public void GetContentTypeShouldReturnImageJpegForImageJpegEnum()
        {
            string result = FamilyContentTypes.Image_JPEG.GetContentType();
            Assert.That(result, Is.EqualTo("image/jpeg"));
        }

        [Test]
        public void GetContentTypeShouldConvertEveryDefinedEnumValueWithoutThrowing()
        {
            foreach (FamilyContentTypes contentType in Enum.GetValues<FamilyContentTypes>())
            {
                Assert.DoesNotThrow(() => contentType.GetContentType());
            }
        }

        // =====================================================================
        // GetContentType(string) — MIME string to enum
        // =====================================================================

        [Test]
        public void GetContentTypeShouldReturnApplicationPdfEnumForApplicationPdfString()
        {
            FamilyContentTypes result = "application/pdf".GetContentType();
            Assert.That(result, Is.EqualTo(FamilyContentTypes.Application_PDF));
        }

        [Test]
        public void GetContentTypeShouldReturnImageJpegEnumForImageJpegString()
        {
            FamilyContentTypes result = "image/jpeg".GetContentType();
            Assert.That(result, Is.EqualTo(FamilyContentTypes.Image_JPEG));
        }

        [Test]
        public void GetContentTypeShouldThrowNotSupportedExceptionForUnsupportedContentTypeString()
        {
            Assert.That(() => "image/png".GetContentType(), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void GetContentTypeShouldThrowNotSupportedExceptionForEmptyContentTypeString()
        {
            Assert.That(() => "".GetContentType(), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void GetContentTypeShouldThrowNotSupportedExceptionForCaseMismatchedContentTypeString()
        {
            Assert.That(() => "Application/PDF".GetContentType(), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void GetContentTypeExceptionMessageShouldIncludeOffendingContentType()
        {
            NotSupportedException? exception = Assert.Throws<NotSupportedException>(() => "image/png".GetContentType());
            Assert.That(exception!.Message, Does.Contain("image/png"));
        }

        // =====================================================================
        // GetContentType — round trip
        // =====================================================================

        [Test]
        public void GetContentTypeShouldRoundTripEveryDefinedEnumValueThroughStringAndBack()
        {
            foreach (FamilyContentTypes contentType in Enum.GetValues<FamilyContentTypes>())
            {
                string mimeType = contentType.GetContentType();
                FamilyContentTypes result = mimeType.GetContentType();
                Assert.That(result, Is.EqualTo(contentType));
            }
        }
    }
}
