using VirtualFamilyMuseumLibrary.Drive;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Models;

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

        // =====================================================================
        // AsTemplateLine(string)
        // =====================================================================

        [Test]
        public void AsTemplateLineShouldThrowInvalidCastExceptionForEmptyLine()
        {
            Assert.That(() => "".AsTemplateLine(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void AsTemplateLineShouldThrowInvalidCastExceptionForWhitespaceOnlyLine()
        {
            Assert.That(() => "   ".AsTemplateLine(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void AsTemplateLineShouldParseMemberWithNoDatesAndNoInLaw()
        {
            TemplateLine result = "1.1) Kit Dale Kessler (–)".AsTemplateLine();

            Assert.That(result.Coordinate, Is.EqualTo(new HierarchicalCoordinate([1, 1])));
            Assert.That(result.MemberBirthName, Is.EqualTo("Kit Dale Kessler"));
            Assert.That(result.MemberBirthDate, Is.Null);
            Assert.That(result.MemberDeceasedDate, Is.Null);
            Assert.That(result.InLawBirthName, Is.Null);
            Assert.That(result.FamilyDynamicStartDate, Is.Null);
        }

        [Test]
        public void AsTemplateLineShouldParseMemberWithBirthDateButNoDeceasedDateAndNoInLaw()
        {
            TemplateLine result = "1) Todd Solo (1975 – Present)".AsTemplateLine();

            Assert.That(result.Coordinate, Is.EqualTo(new HierarchicalCoordinate([1])));
            Assert.That(result.MemberBirthName, Is.EqualTo("Todd Solo"));
            Assert.That(result.MemberBirthDate, Is.EqualTo(new FamilyDate("1975")));
            Assert.That(result.MemberDeceasedDate, Is.Null);
        }

        [Test]
        public void AsTemplateLineShouldParseMemberWithBothDatesAndNoInLaw()
        {
            TemplateLine result = "2) Colby Bryan Kessler (20 Feb 1888 – Apr 1942)".AsTemplateLine();

            Assert.That(result.Coordinate, Is.EqualTo(new HierarchicalCoordinate([2])));
            Assert.That(result.MemberBirthName, Is.EqualTo("Colby Bryan Kessler"));
            Assert.That(result.MemberBirthDate, Is.EqualTo(new FamilyDate("1888", Month.Feb, 20)));
            Assert.That(result.MemberDeceasedDate, Is.EqualTo(new FamilyDate("1942", Month.Apr)));
        }

        [Test]
        public void AsTemplateLineShouldParseFullLineWithInLawAndFamilyDynamicStartDate()
        {
            TemplateLine result = "1) Brian Bryan Kessler (Sep 1886 – Dec 1915) & Todd Zachary Vasterling (1911 – Present): 1948".AsTemplateLine();

            Assert.That(result.Coordinate, Is.EqualTo(new HierarchicalCoordinate([1])));
            Assert.That(result.MemberBirthName, Is.EqualTo("Brian Bryan Kessler"));
            Assert.That(result.MemberBirthDate, Is.EqualTo(new FamilyDate("1886", Month.Sep)));
            Assert.That(result.MemberDeceasedDate, Is.EqualTo(new FamilyDate("1915", Month.Dec)));
            Assert.That(result.InLawBirthName, Is.EqualTo("Todd Zachary Vasterling"));
            Assert.That(result.InLawBirthDate, Is.EqualTo(new FamilyDate("1911")));
            Assert.That(result.InLawDeceasedDate, Is.Null);
            Assert.That(result.FamilyDynamicStartDate, Is.EqualTo(new FamilyDate("1948")));
        }

        [Test]
        public void AsTemplateLineShouldParseInLawWithNoFamilyDynamicStartDate()
        {
            // Regression: this is the exact shape that used to throw ArgumentOutOfRangeException
            // before the family-dynamic-separator-index check was fixed from "< -1" to "< 0".
            TemplateLine result = "1.1.3.1) Quinn Mitchell Thornwood (12 Jan 1990 – Present) & Rhonda Lillian Hollenbeck (10 Nov 1992 – Present)".AsTemplateLine();

            Assert.That(result.Coordinate, Is.EqualTo(new HierarchicalCoordinate([1, 1, 3, 1])));
            Assert.That(result.MemberBirthName, Is.EqualTo("Quinn Mitchell Thornwood"));
            Assert.That(result.MemberBirthDate, Is.EqualTo(new FamilyDate("1990", Month.Jan, 12)));
            Assert.That(result.MemberDeceasedDate, Is.Null);
            Assert.That(result.InLawBirthName, Is.EqualTo("Rhonda Lillian Hollenbeck"));
            Assert.That(result.InLawBirthDate, Is.EqualTo(new FamilyDate("1992", Month.Nov, 10)));
            Assert.That(result.InLawDeceasedDate, Is.Null);
            Assert.That(result.FamilyDynamicStartDate, Is.Null);
        }

        [Test]
        public void AsTemplateLineShouldParseInLawWithNoDatesAtAll()
        {
            TemplateLine result = "3) Steven Chris Kessler (–) & Chris Colby Renquist (–)".AsTemplateLine();

            Assert.That(result.MemberBirthDate, Is.Null);
            Assert.That(result.MemberDeceasedDate, Is.Null);
            Assert.That(result.InLawBirthName, Is.EqualTo("Chris Colby Renquist"));
            Assert.That(result.InLawBirthDate, Is.Null);
            Assert.That(result.InLawDeceasedDate, Is.Null);
        }

        [Test]
        public void AsTemplateLineShouldParseCoordinatesWithManySegments()
        {
            TemplateLine result = "2.4.1.3.3) Danielle Nicole Brandvold (12 May 2020 – Present)".AsTemplateLine();

            Assert.That(result.Coordinate, Is.EqualTo(new HierarchicalCoordinate([2, 4, 1, 3, 3])));
        }

        [Test]
        public void AsTemplateLineShouldThrowFormatExceptionWhenNoHierarchicalCoordinateIsPresent()
        {
            // The leading segment (everything before the first delimiter) is always parsed as
            // the coordinate. If a line doesn't start with one, that segment is non-numeric text
            // and int.Parse surfaces that as a FormatException rather than a domain-specific one.
            Assert.That(() => "John Doe (1975 – Present)".AsTemplateLine(), Throws.TypeOf<FormatException>());
        }

        [Test]
        public void AsTemplateLineShouldThrowInvalidCastExceptionWhenFamilyDynamicStartDateExistsWithoutInLaw()
        {
            InvalidCastException? exception = Assert.Throws<InvalidCastException>(() => "1) Solo Name (1975 – Present): 1943".AsTemplateLine());
            Assert.That(exception!.Message, Does.Contain("in-law"));
        }

        [Test]
        public void AsTemplateLineShouldThrowInvalidCastExceptionWhenFamilyDynamicStartDatePrecedesInLawSummary()
        {
            InvalidCastException? exception = Assert.Throws<InvalidCastException>(() => "1) Member A (1975 – Present): 1943 & In Law B (1980 – Present)".AsTemplateLine());
            Assert.That(exception!.Message, Does.Contain("must come before"));
        }

        [Test]
        public void AsTemplateLineShouldRoundTripThroughToString()
        {
            string[] lines =
            [
                "1.1.2.1.1) Dalon Brandon Kowalczyk (–)",
                "1.1.1) Landon Erik Thornwood (13 Dec 1973 – Present)",
                "1) Lee Ann Alayna Thornwood (2 Dec 1905 – 6 Dec 1966) & Mary Malinda Hollenbeck, Sr. (8 Dec 1925 – Present): 16 Nov 1943",
                "1.1.3.1) Quinn Mitchell Thornwood (12 Jan 1990 – Present) & Rhonda Lillian Hollenbeck (10 Nov 1992 – Present)",
            ];

            foreach (string line in lines)
            {
                TemplateLine result = line.AsTemplateLine();
                Assert.That(result.ToString(), Is.EqualTo(line));
            }
        }

        // =====================================================================
        // PackPages(IEnumerable<IList<string>>)
        // =====================================================================

        [Test]
        public void PackPagesShouldReturnEmptyCollectionForEmptyInput()
        {
            IEnumerable<string[]> result = DriveExtensions.PackPages([]);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void PackPagesShouldFlushASinglePartiallyFilledPageWhenInputNeverReachesCapacity()
        {
            // Regression: an earlier implementation only flushed a page when the *next* group
            // caused an overflow, so input that never overflowed produced zero pages.
            string[] group = ["Line 0", "Line 1", "Line 2"];

            string[][] result = [.. DriveExtensions.PackPages([group])];

            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result[0], Has.Length.EqualTo(49));
            Assert.That(result[0][..3], Is.EqualTo(group));
            Assert.That(result[0][3..], Is.All.Null);
        }

        [Test]
        public void PackPagesShouldPlaceMultipleGroupsSequentiallyOnTheSamePageWhenTheyFit()
        {
            string[] groupA = ["A0", "A1"];
            string[] groupB = ["B0"];

            string[][] result = [.. DriveExtensions.PackPages([groupA, groupB])];

            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result[0][0], Is.EqualTo("A0"));
            Assert.That(result[0][1], Is.EqualTo("A1"));
            Assert.That(result[0][2], Is.EqualTo("B0"));
            Assert.That(result[0][3..], Is.All.Null);
        }

        [Test]
        public void PackPagesShouldNotStartANewPageWhenGroupsExactlyFillPageCapacity()
        {
            string[] firstGroup = [.. Enumerable.Range(0, 45).Select(i => $"Filler {i}")];
            string[] secondGroup = ["Tail 0", "Tail 1", "Tail 2", "Tail 3"];

            string[][] result = [.. DriveExtensions.PackPages([firstGroup, secondGroup])];

            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result[0][..45], Is.EqualTo(firstGroup));
            Assert.That(result[0][45..], Is.EqualTo(secondGroup));
        }

        [Test]
        public void PackPagesShouldKeepAGroupTogetherOnTheNextPageRatherThanSplittingItAcrossThePageBoundary()
        {
            // The business rule this exists for: a de-normalized template line's physical
            // lines must all land on the same page. 47 single-line filler groups leave only
            // 2 slots free on the page; a 3-line group can't fit in those 2 slots, so it must
            // move to a fresh page in full rather than splitting 2 lines onto page 1 and the
            // remaining 1 onto page 2.
            string[] fillerLabels = [.. Enumerable.Range(0, 47).Select(i => $"Filler {i}")];
            IEnumerable<IList<string>> fillerGroups = fillerLabels.Select(label => (IList<string>)new[] { label });
            string[] group = ["Group X Line 0", "Group X Line 1", "Group X Line 2"];

            string[][] result = [.. DriveExtensions.PackPages([.. fillerGroups, group])];

            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0][..47], Is.EqualTo(fillerLabels));
            Assert.That(result[0][47..], Is.All.Null);
            Assert.That(result[1][..3], Is.EqualTo(group));
            Assert.That(result[1][3..], Is.All.Null);
        }

        [Test]
        public void PackPagesShouldReturnExactlyTwoPagesWhenGroupsExactlyFillTwoPagesWorthOfCapacity()
        {
            string[] labels = [.. Enumerable.Range(0, 98).Select(i => $"Line {i}")];
            IEnumerable<IList<string>> groups = labels.Select(label => (IList<string>)new[] { label });

            string[][] result = [.. DriveExtensions.PackPages(groups)];

            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(labels[..49]));
            Assert.That(result[1], Is.EqualTo(labels[49..]));
        }

        [Test]
        public void PackPagesShouldThrowArgumentExceptionForAGroupThatExactlyFillsAnEntirePage()
        {
            string[] group = [.. Enumerable.Range(0, 49).Select(i => $"Line {i}")];
            Assert.That(() => DriveExtensions.PackPages([group]), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void PackPagesShouldThrowArgumentExceptionForAGroupExceedingPageCapacity()
        {
            string[] group = [.. Enumerable.Range(0, 60).Select(i => $"Line {i}")];
            Assert.That(() => DriveExtensions.PackPages([group]), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void PackPagesShouldThrowForAnOversizedGroupRegardlessOfItsPositionAmongOtherGroups()
        {
            string[] smallGroup = ["A0"];
            string[] oversizedGroup = [.. Enumerable.Range(0, 50).Select(i => $"Line {i}")];

            Assert.That(() => DriveExtensions.PackPages([smallGroup, oversizedGroup]), Throws.TypeOf<ArgumentException>());
        }

        // =====================================================================
        // WrapTemplateLine(string)
        // =====================================================================

        [Test]
        public void WrapTemplateLineShouldReturnEmptyListForEmptyString()
        {
            IList<string> result = DriveExtensions.WrapTemplateLine("");
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void WrapTemplateLineShouldReturnEmptyListForWhitespaceOnlyString()
        {
            IList<string> result = DriveExtensions.WrapTemplateLine("   ");
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void WrapTemplateLineShouldReturnSingleLineForSingleShortToken()
        {
            IList<string> result = DriveExtensions.WrapTemplateLine("Todd");
            Assert.That(result, Is.EqualTo(new List<string> { "Todd" }));
        }

        [Test]
        public void WrapTemplateLineShouldJoinTokensThatFitOnOneLineWithASingleSpace()
        {
            IList<string> result = DriveExtensions.WrapTemplateLine("Todd Solo");
            Assert.That(result, Is.EqualTo(new List<string> { "Todd Solo" }));
        }

        [Test]
        public void WrapTemplateLineShouldPreserveAFullLineThatFitsWithinMaxWidth()
        {
            string line = "1) Todd Solo (1975 – Present)";
            IList<string> result = DriveExtensions.WrapTemplateLine(line);
            Assert.That(result, Is.EqualTo(new List<string> { line }));
        }

        [Test]
        public void WrapTemplateLineShouldSplitTwoTokensOntoSeparateLinesWhenTheyDontFitTogether()
        {
            string tokenA = new('a', 40);
            string tokenB = new('b', 40);

            IList<string> result = DriveExtensions.WrapTemplateLine($"{tokenA} {tokenB}");

            Assert.That(result, Is.EqualTo(new List<string> { tokenA, tokenB }));
        }

        [Test]
        public void WrapTemplateLineShouldJoinTokensWhenCombinedLengthExactlyEqualsMaxWidth()
        {
            // 37 + 1 (space) + 37 = 75, exactly at MAX_CHARACTERS_PER_LINE.
            string tokenA = new('a', 37);
            string tokenB = new('b', 37);

            IList<string> result = DriveExtensions.WrapTemplateLine($"{tokenA} {tokenB}");

            Assert.That(result, Is.EqualTo(new List<string> { $"{tokenA} {tokenB}" }));
        }

        [Test]
        public void WrapTemplateLineShouldSplitTokensWhenCombinedLengthExceedsMaxWidthByOne()
        {
            // 37 + 1 (space) + 38 = 76, one over MAX_CHARACTERS_PER_LINE.
            string tokenA = new('a', 37);
            string tokenB = new('b', 38);

            IList<string> result = DriveExtensions.WrapTemplateLine($"{tokenA} {tokenB}");

            Assert.That(result, Is.EqualTo(new List<string> { tokenA, tokenB }));
        }

        [Test]
        public void WrapTemplateLineShouldAllowATokenOneCharacterUnderMaxWidth()
        {
            string token = new('a', 74);
            IList<string> result = DriveExtensions.WrapTemplateLine(token);
            Assert.That(result, Is.EqualTo(new List<string> { token }));
        }

        [Test]
        public void WrapTemplateLineShouldThrowArgumentExceptionForATokenExactlyAtMaxWidth()
        {
            string token = new('a', 75);
            Assert.That(() => DriveExtensions.WrapTemplateLine(token), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void WrapTemplateLineShouldThrowArgumentExceptionForATokenExceedingMaxWidth()
        {
            string token = new('a', 100);
            Assert.That(() => DriveExtensions.WrapTemplateLine(token), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void WrapTemplateLineShouldThrowForAnOverLongTokenRegardlessOfItsPositionAmongOtherTokens()
        {
            string tooLong = new('a', 80);
            Assert.That(() => DriveExtensions.WrapTemplateLine($"Todd {tooLong} Solo"), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void WrapTemplateLineShouldWrapARealisticLongTemplateLineOntoMultiplePhysicalLines()
        {
            string line = "1) Brian Bryan Kessler (Sep 1886 – Dec 1915) & Todd Zachary Vasterling (1911 – Present): 1948";

            IList<string> result = DriveExtensions.WrapTemplateLine(line);

            Assert.That(result, Is.EqualTo(new List<string>
            {
                "1) Brian Bryan Kessler (Sep 1886 – Dec 1915) & Todd Zachary Vasterling",
                "(1911 – Present): 1948"
            }));
        }

        [Test]
        public void WrapTemplateLineShouldDropLeadingWhitespaceRatherThanProduceALeadingSpaceOnTheFirstLine()
        {
            // Split() emits an empty-string token for each leading whitespace character, but
            // the "lineBuilder is empty" branch appends an empty token as a no-op, so leading
            // runs of whitespace are silently dropped rather than preserved.
            IList<string> result = DriveExtensions.WrapTemplateLine("  Todd Solo");
            Assert.That(result, Is.EqualTo(new List<string> { "Todd Solo" }));
        }

        [Test]
        public void WrapTemplateLineShouldPreserveEmbeddedConsecutiveWhitespaceExactly()
        {
            // Split() (no arguments) treats each whitespace character as its own delimiter
            // rather than collapsing runs, so N consecutive spaces yield N-1 empty-string
            // tokens between real words. Each empty token still consumes a single-space
            // append during reconstruction, so the run's width ends up preserved.
            IList<string> result = DriveExtensions.WrapTemplateLine("Todd  Solo");
            Assert.That(result, Is.EqualTo(new List<string> { "Todd  Solo" }));
        }

        [Test]
        public void WrapTemplateLineShouldPreserveTrailingWhitespaceOnTheFinalLine()
        {
            IList<string> result = DriveExtensions.WrapTemplateLine("Todd Solo  ");
            Assert.That(result, Is.EqualTo(new List<string> { "Todd Solo  " }));
        }
    }
}
