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

        // =====================================================================
        // GetPdfPageLines(string[])
        // =====================================================================

        [Test]
        public void GetPdfPageLinesShouldReturnEmptyQueueForEmptyInput()
        {
            Queue<string> result = DriveExtensions.GetPdfPageLines([]);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetPdfPageLinesShouldReturnSingleCompleteLineUnchanged()
        {
            Queue<string> result = DriveExtensions.GetPdfPageLines(["1) Todd Solo (1975 – Present)"]);
            Assert.That(result.Dequeue(), Is.EqualTo("1) Todd Solo (1975 – Present)"));
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetPdfPageLinesShouldPreserveOrderForMultipleUnwrappedLines()
        {
            string[] input =
            [
                "1) Todd Solo (1975 – Present)",
                "1.1) Kit Dale Kessler (–)",
                "2) Colby Bryan Kessler (1888 – 1942)",
            ];

            Queue<string> result = DriveExtensions.GetPdfPageLines(input);

            Assert.That(result, Is.EqualTo(new Queue<string>(input)));
        }

        [Test]
        public void GetPdfPageLinesShouldMergeContinuationLineOntoPreviousEntry()
        {
            string[] input =
            [
                "1) Lee Ann Thornwood (2 Dec 1905 – 6 Dec 1966) & Mary Hollenbeck (8 Dec 1925 –",
                "Present): 16 Nov 1943",
            ];

            Queue<string> result = DriveExtensions.GetPdfPageLines(input);

            Assert.That(result.Dequeue(), Is.EqualTo("1) Lee Ann Thornwood (2 Dec 1905 – 6 Dec 1966) & Mary Hollenbeck (8 Dec 1925 – Present): 16 Nov 1943"));
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetPdfPageLinesShouldMergeMultipleConsecutiveContinuationLinesOntoOneEntry()
        {
            // Regression: "1950)" is coordinate-shaped (digits + ")") but is really the
            // close of a wrapped date, not a new header. Requiring a name (whitespace +
            // letter) after the ")" is what tells the two apart.
            string[] input =
            [
                "1) Some Very Long Name (1900 –",
                "1950) & Another Long Name (1901 –",
                "1955): 1925",
            ];

            Queue<string> result = DriveExtensions.GetPdfPageLines(input);

            Assert.That(result.Dequeue(), Is.EqualTo("1) Some Very Long Name (1900 – 1950) & Another Long Name (1901 – 1955): 1925"));
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetPdfPageLinesShouldKeepEntriesAfterAWrappedEntryIntact()
        {
            // Regression: an earlier implementation tracked merges and removals across two
            // separate, desynced collections, which silently corrupted or dropped entries
            // that followed a wrapped line.
            string[] input =
            [
                "1) Lee Ann Thornwood (2 Dec 1905 – 6 Dec 1966) & Mary Hollenbeck (8 Dec 1925 –",
                "Present): 16 Nov 1943",
                "1.1) Shelby Erika Thornwood (1949 – Present) & Kathy Aldous (18 Aug 1949 –",
                "22 Apr 1983): Oct 1980",
                "1.1.1) Landon Erik Thornwood (13 Dec 1973 – Present)",
            ];

            Queue<string> result = DriveExtensions.GetPdfPageLines(input);

            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result.Dequeue(), Is.EqualTo("1) Lee Ann Thornwood (2 Dec 1905 – 6 Dec 1966) & Mary Hollenbeck (8 Dec 1925 – Present): 16 Nov 1943"));
            Assert.That(result.Dequeue(), Is.EqualTo("1.1) Shelby Erika Thornwood (1949 – Present) & Kathy Aldous (18 Aug 1949 – 22 Apr 1983): Oct 1980"));
            Assert.That(result.Dequeue(), Is.EqualTo("1.1.1) Landon Erik Thornwood (13 Dec 1973 – Present)"));
        }

        [Test]
        public void GetPdfPageLinesShouldTrimTrailingWhitespaceFromUnwrappedLines()
        {
            Queue<string> result = DriveExtensions.GetPdfPageLines(["1.1.1) Landon Erik Thornwood (13 Dec 1973 – Present) "]);

            Assert.That(result.Dequeue(), Is.EqualTo("1.1.1) Landon Erik Thornwood (13 Dec 1973 – Present)"));
        }

        [Test]
        public void GetPdfPageLinesShouldCollapseWhitespaceAtTheWrapBoundaryToASingleSpace()
        {
            string[] input =
            [
                "1) Name (1900 –   ",
                "   Present)",
            ];

            Queue<string> result = DriveExtensions.GetPdfPageLines(input);

            Assert.That(result.Dequeue(), Is.EqualTo("1) Name (1900 – Present)"));
        }

        [Test]
        public void GetPdfPageLinesShouldRecognizeCoordinatesWithManySegments()
        {
            Queue<string> result = DriveExtensions.GetPdfPageLines(["2.4.1.3.3) Danielle Nicole Brandvold (12 May 2020 – Present)"]);

            Assert.That(result.Dequeue(), Is.EqualTo("2.4.1.3.3) Danielle Nicole Brandvold (12 May 2020 – Present)"));
        }

        [Test]
        public void GetPdfPageLinesShouldRecognizeADoubleDigitTopLevelCoordinateAsANewEntry()
        {
            string[] input =
            [
                "9) Ninth Entry Name (1900 – Present)",
                "10) Tenth Entry Name (1901 – Present)",
            ];

            Queue<string> result = DriveExtensions.GetPdfPageLines(input);

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Dequeue(), Is.EqualTo("9) Ninth Entry Name (1900 – Present)"));
            Assert.That(result.Dequeue(), Is.EqualTo("10) Tenth Entry Name (1901 – Present)"));
        }

        [Test]
        public void GetPdfPageLinesShouldRecognizeADoubleDigitNestedCoordinateAsANewEntry()
        {
            string[] input =
            [
                "1.9) Ninth Child Name (1900 – Present)",
                "1.10) Tenth Child Name (1901 – Present)",
            ];

            Queue<string> result = DriveExtensions.GetPdfPageLines(input);

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Dequeue(), Is.EqualTo("1.9) Ninth Child Name (1900 – Present)"));
            Assert.That(result.Dequeue(), Is.EqualTo("1.10) Tenth Child Name (1901 – Present)"));
        }

        [Test]
        public void GetPdfPageLinesShouldTreatAYearClosingAFamilyDynamicStartDateAsAContinuationLine()
        {
            // Regression, taken directly from test-template-1.pdf: a wrapped line can start
            // with just a year closing the family-dynamic-start-date's coordinate-shaped
            // prefix (e.g. "2002):"), which must still be merged rather than treated as a
            // new header.
            string[] input =
            [
                "2.1) Amanda Ryleigh Thornwood (24 Jun 1963 – 24 Jun 1963) & Todd Logan Overby (10 Oct 1945 – 8 Sep",
                "2002): 1987",
            ];

            Queue<string> result = DriveExtensions.GetPdfPageLines(input);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.Dequeue(), Is.EqualTo("2.1) Amanda Ryleigh Thornwood (24 Jun 1963 – 24 Jun 1963) & Todd Logan Overby (10 Oct 1945 – 8 Sep 2002): 1987"));
        }
    }
}
