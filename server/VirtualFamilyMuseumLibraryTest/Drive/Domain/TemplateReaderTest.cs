using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtualFamilyMuseumLibrary;
using VirtualFamilyMuseumLibrary.Drive;
using VirtualFamilyMuseumLibrary.Drive.Domain;
using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Drive.Repository;
using VirtualFamilyMuseumLibrary.Models;

namespace VirtualFamilyMuseumLibraryTest.Drive.Domain
{
    // Integration tests against the real "family6f26m763wyjwkdrive" Azure Storage account,
    // exercising TemplateReader.ReadTemplateAsync end to end (FamilyDrive -> GetPdfPageLines ->
    // AsTemplateLine) rather than mocking any piece of that pipeline. Bootstraps via the generic
    // host the same way FamilyDriveTest/ExtensionsTest do, pulling FamilyDrive configuration from
    // the Test Azure App Configuration store. Requires an Azure identity (e.g. `az login`) with
    // Storage Blob Data Owner/Contributor access on the templates container.
    public class TemplateReaderTest
    {
        // Seeded test data: the same 3 real PDF template blobs FamilyDriveTest reads. Read-only
        // fixtures for this suite and must never be modified or deleted here.
        private const string KESSLER_TEMPLATE_BLOB_NAME = "templates/2026/Aug/14/Kessler#266518394";
        private const string THORNWOOD_TEMPLATE_BLOB_NAME = "templates/2026/Aug/14/Thornwood#1030668903";
        private const string VANTONGEREN_TEMPLATE_BLOB_NAME = "templates/2026/Aug/14/Vantongeren#1545226455";

        private IHost host = null!;
        private TemplateReader reader = null!;
        private IFamilyDriveRepository drive = null!;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Environment.SetEnvironmentVariable("FamilyConfigurationTest__Endpoint", ExtensionsTest.FAMILY_CONFIGURATION_TEST_ENDPOINT);
            Environment.SetEnvironmentVariable("FamilyVaultTest__Endpoint", ExtensionsTest.FAMILY_VAULT_TEST_ENDPOINT);

            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.Configuration.AddConstantStorePipeline(ExecutionTypes.Test);
            builder.AddFamilyInsights(ExecutionTypes.Test);
            builder.AddFamilyDrive();
            host = builder.Build();
            reader = host.Services.GetRequiredService<TemplateReader>();
            drive = host.Services.GetRequiredService<IFamilyDriveRepository>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            host.Dispose();
        }

        // =====================================================================
        // ReadTemplateAsync — seeded template blobs (read-only), full enumeration in order
        // =====================================================================

        [Test]
        public async Task ReadTemplateAsyncShouldEnumerateKesslerTemplateLinesInOrder()
        {
            IEnumerable<TemplateLine> result = await reader.ReadTemplateAsync(KESSLER_TEMPLATE_BLOB_NAME);
            Assert.That(result.ToArray(), Is.EqualTo(KESSLER_TEMPLATE_LINES));
        }

        [Test]
        public async Task ReadTemplateAsyncShouldEnumerateThornwoodTemplateLinesInOrder()
        {
            IEnumerable<TemplateLine> result = await reader.ReadTemplateAsync(THORNWOOD_TEMPLATE_BLOB_NAME);
            Assert.That(result.ToArray(), Is.EqualTo(THORNWOOD_TEMPLATE_LINES));
        }

        [Test]
        public async Task ReadTemplateAsyncShouldEnumerateVantongerenTemplateLinesInOrder()
        {
            IEnumerable<TemplateLine> result = await reader.ReadTemplateAsync(VANTONGEREN_TEMPLATE_BLOB_NAME);
            Assert.That(result.ToArray(), Is.EqualTo(VANTONGEREN_TEMPLATE_LINES));
        }

        // =====================================================================
        // ReadTemplateAsync — not found / wrong content type
        // =====================================================================

        [Test]
        public void ReadTemplateAsyncShouldThrowInvalidOperationExceptionForNonExistentBlob()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () => await reader.ReadTemplateAsync("templates/2026/Aug/14/DoesNotExist.pdf"));
        }

        [Test]
        public async Task ReadTemplateAsyncShouldThrowInvalidOperationExceptionForNonPdfBlob()
        {
            string blobName = $"templates/integration-test-{Guid.NewGuid()}.jpg";
            try
            {
                using MemoryStream upload = new([0xFF, 0xD8, 0xFF, 0xE0]);
                await drive.SaveAsync(blobName, upload, FamilyContentTypes.Image_JPEG);

                Assert.ThrowsAsync<InvalidOperationException>(async () => await reader.ReadTemplateAsync(blobName));
            }
            finally
            {
                await drive.DeleteAsync(blobName);
            }
        }

        // =====================================================================
        // Expected template lines — captured from a real read against the seeded blobs above,
        // via a one-off codegen pass in VirtualFamilyMuseumScratch/Program.cs.
        // =====================================================================

        // KESSLER_TEMPLATE_LINES: 52 lines
        private static readonly TemplateLine[] KESSLER_TEMPLATE_LINES =
        [
            new()
            {
                Coordinate = new([1]),
                MemberBirthName = "Brian Bryan Kessler",
                MemberBirthDate = new("1886", Month.Sep),
                MemberDeceasedDate = new("1915", Month.Dec),
                InLawBirthName = "Todd Zachary Vasterling",
                InLawBirthDate = new("1911"),
                FamilyDynamicStartDate = new("1948"),
            },
            new()
            {
                Coordinate = new([1,1]),
                MemberBirthName = "Kit Dale Kessler",
            },
            new()
            {
                Coordinate = new([1,2]),
                MemberBirthName = "Lisa Kaylynn Kessler",
                InLawBirthName = "Hallie Jordon Delacroix",
                InLawBirthDate = new("1927", Month.Jan, 21),
                FamilyDynamicStartDate = new("1967", Month.May, 11),
            },
            new()
            {
                Coordinate = new([1,2,1]),
                MemberBirthName = "Kevin Nathaniel Kessler",
                MemberBirthDate = new("1949", Month.Jan, 21),
                InLawBirthName = "August Hallie Aldous nee Bordt",
                InLawBirthDate = new("1951", Month.Oct, 9),
                FamilyDynamicStartDate = new("1975", Month.May, 22),
            },
            new()
            {
                Coordinate = new([1,2,1,1]),
                MemberBirthName = "Daric Kit Kessler",
                MemberBirthDate = new("1985", Month.Jun, 18),
                InLawBirthName = "Brooke Shelby Kilbride",
                InLawBirthDate = new("1975", Month.Nov, 12),
            },
            new()
            {
                Coordinate = new([1,2,1,1,1]),
                MemberBirthName = "Jordan Jamie Kessler",
                MemberBirthDate = new("2001", Month.Jul, 6),
                MemberDeceasedDate = new("2025", Month.Jun, 28),
            },
            new()
            {
                Coordinate = new([1,2,1,1,2]),
                MemberBirthName = "Beth Amanda Kessler",
                MemberBirthDate = new("2006", Month.Nov, 10),
            },
            new()
            {
                Coordinate = new([1,2,1,1,3]),
                MemberBirthName = "Krista Nicole Kessler",
                MemberBirthDate = new("1994", Month.Jun, 21),
            },
            new()
            {
                Coordinate = new([1,2,1,2]),
                MemberBirthName = "Jordon Conrad Kessler",
                MemberBirthDate = new("1985", Month.Jun, 18),
                InLawBirthName = "Frieda Laurel Delacroix",
                InLawBirthDate = new("1971", Month.Apr, 21),
                InLawDeceasedDate = new("2002", Month.Nov, 14),
                FamilyDynamicStartDate = new("2001", Month.Aug, 14),
            },
            new()
            {
                Coordinate = new([1,2,1,2,1]),
                MemberBirthName = "Kaylynn Kevin Kessler",
                MemberBirthDate = new("1996", Month.Jul, 24),
            },
            new()
            {
                Coordinate = new([1,2,1,2,2]),
                MemberBirthName = "Daniel Paul Kessler, IV",
                MemberBirthDate = new("1996", Month.Jul, 24),
            },
            new()
            {
                Coordinate = new([1,2,2]),
                MemberBirthName = "Lanelle Madilynn Kessler",
                MemberBirthDate = new("1960", Month.Dec, 12),
            },
            new()
            {
                Coordinate = new([1,2,3]),
                MemberBirthName = "Logan Conrad Kessler",
                MemberBirthDate = new("1961", Month.Nov, 14),
                InLawBirthName = "Eloise Lorna Thornwood nee Schulz",
                InLawBirthDate = new("1953", Month.Apr, 28),
                InLawDeceasedDate = new("1977", Month.Apr, 12),
                FamilyDynamicStartDate = new("1981"),
            },
            new()
            {
                Coordinate = new([1,2,3,1]),
                MemberBirthName = "Kaylynn Nathan Kessler",
                MemberBirthDate = new("1975", Month.Jun, 2),
                InLawBirthName = "Gordon Jordan Nordstrom",
                InLawBirthDate = new("1977", Month.Jul, 11),
                InLawDeceasedDate = new("2012", Month.Apr, 24),
                FamilyDynamicStartDate = new("2004"),
            },
            new()
            {
                Coordinate = new([1,2,3,1,1]),
                MemberBirthName = "Elwood Tyson Kessler",
                MemberBirthDate = new("2003"),
                MemberDeceasedDate = new("2009", Month.Feb, 24),
            },
            new()
            {
                Coordinate = new([1,2,3,2]),
                MemberBirthName = "Lanelle Ella Kessler",
                MemberBirthDate = new("1981", Month.Jan, 3),
                MemberDeceasedDate = new("2012", Month.Jul, 17),
                InLawBirthName = "Lillian Lee Ann Kilbride",
                InLawBirthDate = new("1974", Month.May, 17),
                InLawDeceasedDate = new("2009", Month.Nov, 21),
                FamilyDynamicStartDate = new("1997", Month.Mar, 16),
            },
            new()
            {
                Coordinate = new([1,2,3,3]),
                MemberBirthName = "Madilynn Heidi Kessler",
                MemberBirthDate = new("1985", Month.Mar, 23),
            },
            new()
            {
                Coordinate = new([1,2,3,4]),
                MemberBirthName = "Sawyer Kevin Kessler",
                MemberBirthDate = new("1976", Month.Mar, 25),
                MemberDeceasedDate = new("2025", Month.Jan),
                InLawBirthName = "Jodi Jamie Hallberg nee Tufty",
                InLawBirthDate = new("1971", Month.Jul),
                FamilyDynamicStartDate = new("1996", Month.Nov, 15),
            },
            new()
            {
                Coordinate = new([1,2,3,4,1]),
                MemberBirthName = "Britny Kiyah Kessler",
                MemberBirthDate = new("2000", Month.Apr, 17),
                MemberDeceasedDate = new("2023"),
            },
            new()
            {
                Coordinate = new([1,2,3,4,2]),
                MemberBirthName = "Eloise Leva Kessler",
                MemberBirthDate = new("1997", Month.Jun, 3),
            },
            new()
            {
                Coordinate = new([1,2,3,4,3]),
                MemberBirthName = "Conrad Robert Kessler",
                MemberBirthDate = new("2011", Month.Oct, 10),
                MemberDeceasedDate = new("2021"),
            },
            new()
            {
                Coordinate = new([1,2,3,4,4]),
                MemberBirthName = "Lee Ann Emily Kessler",
                MemberBirthDate = new("2002"),
                MemberDeceasedDate = new("2022", Month.Mar, 13),
            },
            new()
            {
                Coordinate = new([1,2,3,4,5]),
                MemberBirthName = "Daric Emmitt Kessler",
                MemberBirthDate = new("1996", Month.Nov, 24),
                MemberDeceasedDate = new("2017"),
            },
            new()
            {
                Coordinate = new([1,2,3,5]),
                MemberBirthName = "Dorothy Kathrin Kessler",
                MemberBirthDate = new("1980", Month.Apr, 16),
            },
            new()
            {
                Coordinate = new([1,2,4]),
                MemberBirthName = "Olivia Ryleigh Kessler",
                MemberBirthDate = new("1949", Month.Dec, 27),
                InLawBirthName = "Darla Brinley Bergstrom",
                InLawBirthDate = new("1955", Month.Aug, 23),
                InLawDeceasedDate = new("2000", Month.Sep, 7),
                FamilyDynamicStartDate = new("1991", Month.Mar),
            },
            new()
            {
                Coordinate = new([1,2,4,1]),
                MemberBirthName = "Zachary Jason Kessler",
                MemberBirthDate = new("1978", Month.May, 21),
            },
            new()
            {
                Coordinate = new([1,2,4,2]),
                MemberBirthName = "Keith Hallie Kessler",
                MemberBirthDate = new("1986", Month.Jan),
                MemberDeceasedDate = new("2018", Month.Oct),
            },
            new()
            {
                Coordinate = new([1,3]),
                MemberBirthName = "Kit Hallie Kessler",
                MemberBirthDate = new("1937"),
                MemberDeceasedDate = new("2013", Month.Apr, 4),
            },
            new()
            {
                Coordinate = new([2]),
                MemberBirthName = "Colby Bryan Kessler",
                MemberBirthDate = new("1888", Month.Feb, 20),
                MemberDeceasedDate = new("1942", Month.Apr),
                InLawBirthName = "Gladys Samantha Tufty",
                InLawBirthDate = new("1909", Month.Feb, 3),
                InLawDeceasedDate = new("1940-1943", Month.Sep),
                FamilyDynamicStartDate = new("1941", Month.Apr, 25),
            },
            new()
            {
                Coordinate = new([2,1]),
                MemberBirthName = "Dorothy Ryleigh Kessler",
                MemberBirthDate = new("1936"),
                MemberDeceasedDate = new("1991", Month.Dec, 13),
                InLawBirthName = "Ella Nicole Kessler",
                InLawBirthDate = new("1929"),
                InLawDeceasedDate = new("1961", Month.Apr, 17),
            },
            new()
            {
                Coordinate = new([2,1,1]),
                MemberBirthName = "Steven Tanner Kessler II",
                MemberBirthDate = new("1950", Month.Jun),
                MemberDeceasedDate = new("1972", Month.Jan),
            },
            new()
            {
                Coordinate = new([2,1,2]),
                MemberBirthName = "Rhonda Brinley Kessler",
                MemberBirthDate = new("1960", Month.Dec, 23),
                MemberDeceasedDate = new("2000", Month.Sep, 17),
                InLawBirthName = "Darin Brandon Vantongeren",
                InLawBirthDate = new("1951", Month.May, 24),
                InLawDeceasedDate = new("1973", Month.Aug, 11),
                FamilyDynamicStartDate = new("1981", Month.Apr, 4),
            },
            new()
            {
                Coordinate = new([2,1,2,1]),
                MemberBirthName = "Conrad Tyson Kessler",
                MemberBirthDate = new("1971", Month.Apr, 2),
                InLawBirthName = "Taryn Lori Kilbride",
                InLawBirthDate = new("1976", Month.Apr, 6),
                InLawDeceasedDate = new("2014", Month.Mar, 18),
                FamilyDynamicStartDate = new("1997", Month.Aug, 21),
            },
            new()
            {
                Coordinate = new([2,1,2,1,1]),
                MemberBirthName = "Rhonda Miranda Kessler",
                MemberBirthDate = new("2007", Month.Jan, 5),
                MemberDeceasedDate = new("2015", Month.May, 16),
            },
            new()
            {
                Coordinate = new([2,1,2,1,2]),
                MemberBirthName = "Laurel \"Pete\" Beth Kessler",
                MemberBirthDate = new("2002", Month.Mar, 22),
            },
            new()
            {
                Coordinate = new([2,1,2,1,3]),
                MemberBirthName = "Gladys Marcie Kessler",
                MemberBirthDate = new("2004", Month.Oct, 10),
                MemberDeceasedDate = new("2016", Month.Mar, 21),
            },
            new()
            {
                Coordinate = new([2,1,2,1,4]),
                MemberBirthName = "Jordan Danielle Kessler II",
                MemberBirthDate = new("1998", Month.Jun, 23),
                MemberDeceasedDate = new("2016", Month.Jan, 9),
            },
            new()
            {
                Coordinate = new([2,1,2,1,5]),
                MemberBirthName = "Leon Emmitt Bergstrom",
                MemberBirthDate = new("2011", Month.Mar, 28),
                MemberDeceasedDate = new("2015"),
            },
            new()
            {
                Coordinate = new([2,1,2,2]),
                MemberBirthName = "Gladys Gladys Kessler",
                MemberBirthDate = new("1974", Month.Jul, 3),
                MemberDeceasedDate = new("2024", Month.Apr, 12),
            },
            new()
            {
                Coordinate = new([2,1,3]),
                MemberBirthName = "Jovey Kathy Kessler",
                MemberBirthDate = new("1955", Month.Aug, 4),
                MemberDeceasedDate = new("1995"),
            },
            new()
            {
                Coordinate = new([2,2]),
                MemberBirthName = "Lanelle Jodi Kessler II",
                MemberBirthDate = new("1930", Month.Apr, 23),
                InLawBirthName = "Jodi \"Bud\" Madilynn Kowalczyk",
                InLawBirthDate = new("1931", Month.Nov, 23),
                FamilyDynamicStartDate = new("1968", Month.Jan, 11),
            },
            new()
            {
                Coordinate = new([2,2,1]),
                MemberBirthName = "Hallie Ricky Kessler",
                MemberBirthDate = new("1958", Month.Nov, 5),
            },
            new()
            {
                Coordinate = new([2,2,2]),
                MemberBirthName = "Brooke Emily Kessler",
                MemberBirthDate = new("1966", Month.Sep, 15),
            },
            new()
            {
                Coordinate = new([2,2,3]),
                MemberBirthName = "Steven Chris Kessler",
                MemberBirthDate = new("1957", Month.Jun, 21),
                MemberDeceasedDate = new("2014", Month.Aug, 5),
                InLawBirthName = "Chris Colby Renquist",
                InLawBirthDate = new("1949", Month.Feb, 22),
                FamilyDynamicStartDate = new("1989", Month.Jun, 21),
            },
            new()
            {
                Coordinate = new([2,2,3,1]),
                MemberBirthName = "Katrina Kiyah Kessler",
                MemberBirthDate = new("1975", Month.May, 8),
                MemberDeceasedDate = new("2005"),
                InLawBirthName = "Alayna Shelby Ashworth nee Dachtler",
                InLawBirthDate = new("1977", Month.Jan, 1),
                InLawDeceasedDate = new("1987", Month.Jul, 25),
                FamilyDynamicStartDate = new("2016"),
            },
            new()
            {
                Coordinate = new([2,2,3,2]),
                MemberBirthName = "Marlin Todd Kessler",
                MemberBirthDate = new("1988", Month.May, 15),
                InLawBirthName = "Victoria Samantha Hallberg",
                InLawBirthDate = new("1975", Month.Dec, 20),
                FamilyDynamicStartDate = new("1991", Month.Feb, 8),
            },
            new()
            {
                Coordinate = new([2,2,3,2,1]),
                MemberBirthName = "Katelyn Hannah Kessler",
                MemberBirthDate = new("2000", Month.Dec, 19),
            },
            new()
            {
                Coordinate = new([2,2,3,2,2]),
                MemberBirthName = "Logan Matthew Kessler",
                MemberBirthDate = new("2000", Month.Dec, 19),
            },
            new()
            {
                Coordinate = new([2,2,3,2,3]),
                MemberBirthName = "Lee Ann Mary Kessler",
                MemberBirthDate = new("2000", Month.Dec, 19),
            },
            new()
            {
                Coordinate = new([2,3]),
                MemberBirthName = "Zachary \"Dutch\" Colby Kessler",
                MemberBirthDate = new("1928", Month.Nov, 20),
            },
            new()
            {
                Coordinate = new([2,4]),
                MemberBirthName = "Gordon Brian Kessler",
                MemberBirthDate = new("1944", Month.May, 27),
            },
            new()
            {
                Coordinate = new([3]),
                MemberBirthName = "Bryan Adam Kessler, Jr.",
                MemberBirthDate = new("1888", Month.Mar, 18),
                MemberDeceasedDate = new("1957", Month.Jul, 17),
                InLawBirthName = "Ricky LeRoy Ashworth",
                InLawBirthDate = new("1907", Month.Jan, 24),
                FamilyDynamicStartDate = new("1934", Month.Jan, 22),
            },
        ];

        // THORNWOOD_TEMPLATE_LINES: 59 lines
        private static readonly TemplateLine[] THORNWOOD_TEMPLATE_LINES =
        [
            new()
            {
                Coordinate = new([1]),
                MemberBirthName = "Lee Ann Alayna Thornwood",
                MemberBirthDate = new("1905", Month.Dec, 2),
                MemberDeceasedDate = new("1966", Month.Dec, 6),
                InLawBirthName = "Mary Malinda Hollenbeck, Sr.",
                InLawBirthDate = new("1925", Month.Dec, 8),
                FamilyDynamicStartDate = new("1943", Month.Nov, 16),
            },
            new()
            {
                Coordinate = new([1,1]),
                MemberBirthName = "Shelby Erika Thornwood",
                MemberBirthDate = new("1949"),
                InLawBirthName = "Kathy Britny Aldous",
                InLawBirthDate = new("1949", Month.Aug, 18),
                InLawDeceasedDate = new("1983", Month.Apr, 22),
                FamilyDynamicStartDate = new("1980", Month.Oct),
            },
            new()
            {
                Coordinate = new([1,1,1]),
                MemberBirthName = "Landon Erik Thornwood",
                MemberBirthDate = new("1973", Month.Dec, 13),
            },
            new()
            {
                Coordinate = new([1,1,2]),
                MemberBirthName = "Nathaniel Todd Thornwood",
                MemberBirthDate = new("1972", Month.Feb, 1),
                InLawBirthName = "Hunter Alex Hallberg",
                InLawBirthDate = new("1970", Month.Dec, 27),
                FamilyDynamicStartDate = new("2003", Month.Aug, 27),
            },
            new()
            {
                Coordinate = new([1,1,2,1]),
                MemberBirthName = "Steven Emmitt Thornwood",
                MemberBirthDate = new("1995", Month.May, 1),
                InLawBirthName = "Jovey Lee Ann Hollenbeck",
                InLawBirthDate = new("1992", Month.Mar),
                FamilyDynamicStartDate = new("2011", Month.Mar, 22),
            },
            new()
            {
                Coordinate = new([1,1,2,1,1]),
                MemberBirthName = "Dalon Brandon Thornwood",
            },
            new()
            {
                Coordinate = new([1,1,2,1,2]),
                MemberBirthName = "Lorna Danielle Thornwood",
                MemberBirthDate = new("2025"),
            },
            new()
            {
                Coordinate = new([1,1,2,2]),
                MemberBirthName = "Elwood Matthew Thornwood",
                MemberBirthDate = new("1990", Month.Dec, 26),
            },
            new()
            {
                Coordinate = new([1,1,2,3]),
                MemberBirthName = "Krista Gladys Thornwood",
                MemberBirthDate = new("2002", Month.Jun, 26),
                InLawBirthName = "Joshua Quinn Kirschbaum nee Linke",
                InLawBirthDate = new("1987", Month.Apr, 15),
                InLawDeceasedDate = new("2005"),
                FamilyDynamicStartDate = new("2007", Month.Dec, 17),
            },
            new()
            {
                Coordinate = new([1,1,2,4]),
                MemberBirthName = "Michael Nathaniel Thornwood",
                MemberBirthDate = new("1992", Month.Jun),
                MemberDeceasedDate = new("2023", Month.Dec, 27),
            },
            new()
            {
                Coordinate = new([1,1,3]),
                MemberBirthName = "Nathaniel Logan Thornwood",
                MemberBirthDate = new("1981", Month.Dec, 10),
                MemberDeceasedDate = new("2026", Month.Jan),
                InLawBirthName = "Sharon Kaylynn Renquist II nee Wetstein",
                InLawBirthDate = new("1970", Month.Oct),
                FamilyDynamicStartDate = new("1985", Month.Sep, 1),
            },
            new()
            {
                Coordinate = new([1,1,3,1]),
                MemberBirthName = "Quinn Mitchell Thornwood",
                MemberBirthDate = new("1990", Month.Jan, 12),
                InLawBirthName = "Debra Amanda Overby",
                InLawBirthDate = new("1987", Month.Sep, 5),
                FamilyDynamicStartDate = new("2022", Month.Jan, 23),
            },
            new()
            {
                Coordinate = new([1,1,3,2]),
                MemberBirthName = "Quinn Mitchell Thornwood",
                MemberBirthDate = new("1990", Month.Jan, 12),
                InLawBirthName = "Rhonda Lillian Hollenbeck",
                InLawBirthDate = new("1992", Month.Nov, 10),
            },
            new()
            {
                Coordinate = new([1,1,3,2,1]),
                MemberBirthName = "Landon LeRoy Thornwood",
                MemberBirthDate = new("2018", Month.Mar, 12),
            },
            new()
            {
                Coordinate = new([1,1,3,3]),
                MemberBirthName = "Darin Paul Thornwood",
                MemberBirthDate = new("1990"),
                MemberDeceasedDate = new("2009", Month.Apr, 20),
            },
            new()
            {
                Coordinate = new([2]),
                MemberBirthName = "Brian Brandon Thornwood",
                MemberBirthDate = new("1907", Month.Aug, 19),
                MemberDeceasedDate = new("1970"),
                InLawBirthName = "Kaylynn Samantha Tufty",
                InLawBirthDate = new("1923", Month.Sep, 3),
                FamilyDynamicStartDate = new("1964"),
            },
            new()
            {
                Coordinate = new([2,1]),
                MemberBirthName = "Amanda Ryleigh Thornwood",
                MemberBirthDate = new("1963", Month.Jun, 24),
                MemberDeceasedDate = new("1963", Month.Jun, 24),
                InLawBirthName = "Todd Logan Overby",
                InLawBirthDate = new("1945", Month.Oct, 10),
                InLawDeceasedDate = new("2002", Month.Sep, 8),
                FamilyDynamicStartDate = new("1987"),
            },
            new()
            {
                Coordinate = new([2,2]),
                MemberBirthName = "Kit Jacob Thornwood",
                MemberBirthDate = new("1963", Month.Jun, 24),
                MemberDeceasedDate = new("1963", Month.Jun, 24),
            },
            new()
            {
                Coordinate = new([2,3]),
                MemberBirthName = "Kiyah Amanda Renquist",
                MemberBirthDate = new("1963", Month.Jun, 24),
                MemberDeceasedDate = new("1963", Month.Jun, 24),
            },
            new()
            {
                Coordinate = new([2,4]),
                MemberBirthName = "Kevin Erik Thornwood",
                MemberBirthDate = new("1963", Month.Jun, 24),
                MemberDeceasedDate = new("1963", Month.Jun, 24),
                InLawBirthName = "Laurel Holly Hallberg nee Wetstein",
                InLawBirthDate = new("1949", Month.Feb, 24),
                FamilyDynamicStartDate = new("1975"),
            },
            new()
            {
                Coordinate = new([2,4,1]),
                MemberBirthName = "Kiyah Hannah Thornwood",
                MemberBirthDate = new("1974", Month.Aug, 19),
                MemberDeceasedDate = new("1974", Month.Aug, 19),
                InLawBirthName = "Marcie Madilynn Ashworth",
                InLawBirthDate = new("1966", Month.Jun, 23),
                FamilyDynamicStartDate = new("2000", Month.Nov, 9),
            },
            new()
            {
                Coordinate = new([2,4,1,1]),
                MemberBirthName = "Paul Sawyer Thornwood",
                MemberBirthDate = new("1999"),
                MemberDeceasedDate = new("2015"),
            },
            new()
            {
                Coordinate = new([2,4,1,2]),
                MemberBirthName = "Emily Payton Thornwood",
                MemberBirthDate = new("1987", Month.Mar, 21),
                MemberDeceasedDate = new("2023", Month.Dec),
                InLawBirthName = "Malinda Dorothy Hollenbeck",
                InLawBirthDate = new("1988"),
                InLawDeceasedDate = new("2013", Month.May, 23),
                FamilyDynamicStartDate = new("2019", Month.Jun),
            },
            new()
            {
                Coordinate = new([2,4,1,2,1]),
                MemberBirthName = "Cordell Jordan Thornwood III",
                MemberBirthDate = new("2016", Month.Feb, 26),
            },
            new()
            {
                Coordinate = new([2,4,1,3]),
                MemberBirthName = "Tanner Terry Thornwood",
                MemberBirthDate = new("1993", Month.Jul, 7),
                MemberDeceasedDate = new("2004", Month.Sep),
                InLawBirthName = "Tyson Conrad Lindqvist",
                InLawBirthDate = new("1992"),
                FamilyDynamicStartDate = new("2017", Month.Jun, 19),
            },
            new()
            {
                Coordinate = new([2,4,1,3,1]),
                MemberBirthName = "Emmitt Marlin Thornwood",
                MemberBirthDate = new("2018", Month.Aug, 20),
            },
            new()
            {
                Coordinate = new([2,4,1,3,2]),
                MemberBirthName = "Shelby Jordan Thornwood",
                MemberBirthDate = new("2021", Month.Apr, 11),
            },
            new()
            {
                Coordinate = new([2,4,1,3,3]),
                MemberBirthName = "Danielle Nicole Thornwood",
                MemberBirthDate = new("2020", Month.May, 12),
            },
            new()
            {
                Coordinate = new([2,4,2]),
                MemberBirthName = "Kinsey Erika Thornwood",
                MemberBirthDate = new("1974", Month.Aug, 19),
                MemberDeceasedDate = new("1974", Month.Aug, 19),
                InLawBirthName = "Adam Tyson Wachtler",
                InLawBirthDate = new("1968"),
                FamilyDynamicStartDate = new("1994", Month.Jan, 24),
            },
            new()
            {
                Coordinate = new([2,4,2,1]),
                MemberBirthName = "Malinda Sharon Wachtler",
                MemberBirthDate = new("2001", Month.Jan, 23),
                InLawBirthName = "Zachary Keith Lindqvist, Jr.",
                InLawBirthDate = new("1988", Month.Aug, 28),
                FamilyDynamicStartDate = new("2026", Month.Feb, 1),
            },
            new()
            {
                Coordinate = new([2,4,2,1,1]),
                MemberBirthName = "Laurel Victoria Lindqvist",
                MemberBirthDate = new("2018", Month.Nov, 27),
            },
            new()
            {
                Coordinate = new([2,4,2,1,2]),
                MemberBirthName = "Kathrin Charley Lindqvist",
                MemberBirthDate = new("2017"),
                MemberDeceasedDate = new("2026", Month.Oct, 12),
            },
            new()
            {
                Coordinate = new([2,4,2,1,3]),
                MemberBirthName = "Jamie Alayna Lindqvist",
                MemberBirthDate = new("2011", Month.Sep, 18),
                MemberDeceasedDate = new("2026-2026", Month.May),
            },
            new()
            {
                Coordinate = new([2,4,2,2]),
                MemberBirthName = "Lanelle Lisa Wachtler",
                MemberBirthDate = new("1989"),
            },
            new()
            {
                Coordinate = new([2,4,2,3]),
                MemberBirthName = "Michael Jacob Wachtler",
                MemberBirthDate = new("1989", Month.Mar, 28),
            },
            new()
            {
                Coordinate = new([2,4,2,4]),
                MemberBirthName = "Victoria Marcie Wachtler",
                MemberBirthDate = new("2001", Month.Feb, 18),
                InLawBirthName = "Bryan \"Bud\" Elwood Hallberg",
                InLawBirthDate = new("1987", Month.Feb, 14),
                FamilyDynamicStartDate = new("2010", Month.Sep, 25),
            },
            new()
            {
                Coordinate = new([2,4,2,4,1]),
                MemberBirthName = "Lori Madilynn Hallberg",
                MemberBirthDate = new("2011", Month.Sep, 7),
            },
            new()
            {
                Coordinate = new([2,4,2,4,2]),
                MemberBirthName = "Katelyn Miranda Hallberg",
                MemberBirthDate = new("2014"),
            },
            new()
            {
                Coordinate = new([2,4,2,4,3]),
                MemberBirthName = "Lori Jodi Hallberg",
                MemberBirthDate = new("2026", Month.Jul, 18),
            },
            new()
            {
                Coordinate = new([2,4,3]),
                MemberBirthName = "Tanner Erik Thornwood",
                MemberBirthDate = new("1974", Month.Aug, 19),
                MemberDeceasedDate = new("1974", Month.Aug, 19),
                InLawBirthName = "Eloise Kinsey Brandvold",
                InLawBirthDate = new("1969"),
                InLawDeceasedDate = new("2007", Month.May, 22),
                FamilyDynamicStartDate = new("2009", Month.Sep, 17),
            },
            new()
            {
                Coordinate = new([2,4,3,1]),
                MemberBirthName = "Kathy Katelyn Thornwood",
                MemberBirthDate = new("1988", Month.May, 1),
                InLawBirthName = "Dalon Jason Falkenberg",
                InLawBirthDate = new("1993"),
                FamilyDynamicStartDate = new("2016", Month.Feb, 26),
            },
            new()
            {
                Coordinate = new([2,4,3,1,1]),
                MemberBirthName = "Jacob Darin Falkenberg",
                MemberBirthDate = new("2009"),
                MemberDeceasedDate = new("2026-2026", Month.Apr),
            },
            new()
            {
                Coordinate = new([2,4,3,1,2]),
                MemberBirthName = "Jeremy Todd Falkenberg",
                MemberBirthDate = new("2021", Month.Aug, 14),
            },
            new()
            {
                Coordinate = new([2,4,3,2]),
                MemberBirthName = "Dorothy Miranda Thornwood",
                MemberBirthDate = new("1999", Month.Oct),
                MemberDeceasedDate = new("2008", Month.Feb, 26),
                InLawBirthName = "Bethany Debra Steinauer",
                InLawBirthDate = new("1987"),
                InLawDeceasedDate = new("2012"),
                FamilyDynamicStartDate = new("2016", Month.Nov, 25),
            },
            new()
            {
                Coordinate = new([2,4,3,2,1]),
                MemberBirthName = "Malinda Charley Thornwood",
                MemberBirthDate = new("2023", Month.May, 2),
            },
            new()
            {
                Coordinate = new([2,4,3,2,2]),
                MemberBirthName = "Morten Tyson Thornwood",
                MemberBirthDate = new("2012"),
                MemberDeceasedDate = new("2024", Month.May, 18),
            },
            new()
            {
                Coordinate = new([2,4,4]),
                MemberBirthName = "Mitchell Kamie Thornwood",
                MemberBirthDate = new("1974", Month.Aug, 19),
                MemberDeceasedDate = new("1974", Month.Aug, 19),
                InLawBirthName = "Erika Britny Kessler, Jr.",
                InLawBirthDate = new("1971", Month.Sep, 3),
                FamilyDynamicStartDate = new("2006", Month.Oct, 14),
            },
            new()
            {
                Coordinate = new([2,4,5]),
                MemberBirthName = "Rhonda Katelyn Thornwood",
                MemberBirthDate = new("1974", Month.Aug, 19),
                MemberDeceasedDate = new("1974", Month.Aug, 19),
                InLawBirthName = "Lillian Leva Steinauer",
                InLawBirthDate = new("1970", Month.Jul, 3),
                FamilyDynamicStartDate = new("2001", Month.Jun, 26),
            },
            new()
            {
                Coordinate = new([2,4,5,1]),
                MemberBirthName = "Jason Tanner Thornwood",
                MemberBirthDate = new("1989", Month.Jul, 7),
                InLawBirthName = "Amanda Marlys Kirschbaum",
                InLawBirthDate = new("1988", Month.Sep, 17),
                InLawDeceasedDate = new("2004", Month.Feb, 22),
                FamilyDynamicStartDate = new("2012", Month.May, 13),
            },
            new()
            {
                Coordinate = new([2,4,5,1,1]),
                MemberBirthName = "Conrad Mitchell Thornwood",
                MemberBirthDate = new("2022", Month.Mar, 23),
            },
            new()
            {
                Coordinate = new([2,4,5,1,2]),
                MemberBirthName = "John Cordell Thornwood",
                MemberBirthDate = new("2022", Month.Mar, 23),
            },
            new()
            {
                Coordinate = new([2,4,5,1,3]),
                MemberBirthName = "Brian Derek Thornwood",
                MemberBirthDate = new("2022", Month.Mar, 23),
            },
            new()
            {
                Coordinate = new([2,4,5,2]),
                MemberBirthName = "Daniel Erik Thornwood",
                MemberBirthDate = new("1987", Month.Jun, 1),
                MemberDeceasedDate = new("2016", Month.Feb, 1),
                InLawBirthName = "Jovey Erika Renquist, III",
                InLawBirthDate = new("1990", Month.May, 3),
                InLawDeceasedDate = new("2013", Month.Aug, 17),
                FamilyDynamicStartDate = new("2020"),
            },
            new()
            {
                Coordinate = new([2,4,5,2,1]),
                MemberBirthName = "Amanda Rhonda Thornwood",
                MemberBirthDate = new("2015", Month.Jul, 17),
            },
            new()
            {
                Coordinate = new([2,4,5,2,2]),
                MemberBirthName = "Erika Miranda Thornwood",
                MemberBirthDate = new("2014", Month.Nov, 3),
                MemberDeceasedDate = new("2024", Month.Nov, 16),
            },
            new()
            {
                Coordinate = new([2,4,5,3]),
                MemberBirthName = "Jordan Kamie Thornwood",
                MemberBirthDate = new("1991", Month.Oct, 5),
                MemberDeceasedDate = new("2014", Month.Feb, 3),
            },
            new()
            {
                Coordinate = new([2,5]),
                MemberBirthName = "Jamie Sheila Thornwood",
                MemberBirthDate = new("1963", Month.Jun, 24),
                MemberDeceasedDate = new("1963", Month.Jun, 24),
            },
            new()
            {
                Coordinate = new([3]),
                MemberBirthName = "Kamie Todd Thornwood",
                MemberBirthDate = new("1901", Month.Dec, 9),
                MemberDeceasedDate = new("1965-1966", Month.Jan),
                InLawBirthName = "Brian Jacob Christmann",
                InLawBirthDate = new("1926"),
                InLawDeceasedDate = new("1987", Month.Jun, 8),
                FamilyDynamicStartDate = new("1952"),
            },
            new()
            {
                Coordinate = new([3,1]),
                MemberBirthName = "Adam Steven Thornwood",
                MemberBirthDate = new("1954", Month.Jan, 10),
                MemberDeceasedDate = new("2021-2023", Month.Sep),
            },
        ];

        // VANTONGEREN_TEMPLATE_LINES: 43 lines
        private static readonly TemplateLine[] VANTONGEREN_TEMPLATE_LINES =
        [
            new()
            {
                Coordinate = new([1]),
                MemberBirthName = "Nathaniel Dak Vantongeren",
                MemberBirthDate = new("1894", Month.Nov, 6),
                MemberDeceasedDate = new("1950", Month.Nov, 4),
                InLawBirthName = "Kathrin Mary Linke",
                InLawBirthDate = new("1913", Month.Sep, 26),
                FamilyDynamicStartDate = new("1944", Month.Nov, 26),
            },
            new()
            {
                Coordinate = new([1,1]),
                MemberBirthName = "Reed Jordan Vantongeren",
                MemberBirthDate = new("1937", Month.Aug, 12),
                MemberDeceasedDate = new("1959-1960", Month.Jan),
                InLawBirthName = "Emily Evelyn Marchetti",
                InLawBirthDate = new("1934", Month.May, 12),
                FamilyDynamicStartDate = new("1954", Month.Apr, 20),
            },
            new()
            {
                Coordinate = new([1,1,1]),
                MemberBirthName = "Daniel Keith Vantongeren",
                MemberBirthDate = new("1964", Month.Sep),
                InLawBirthName = "Brinley Brinley Hallberg III",
                InLawBirthDate = new("1961", Month.Jan, 12),
                FamilyDynamicStartDate = new("1999"),
            },
            new()
            {
                Coordinate = new([1,1,1,1]),
                MemberBirthName = "Hallie Kamie Vantongeren",
                MemberBirthDate = new("1994", Month.Oct, 18),
                MemberDeceasedDate = new("2000"),
                InLawBirthName = "Gladys Evelyn Kirschbaum",
                InLawBirthDate = new("1983", Month.Oct, 13),
                FamilyDynamicStartDate = new("2007", Month.Mar, 7),
            },
            new()
            {
                Coordinate = new([1,1,1,1,1]),
                MemberBirthName = "Eloise Frieda Vantongeren",
                MemberBirthDate = new("2011", Month.Dec, 4),
                MemberDeceasedDate = new("2021", Month.Apr, 16),
            },
            new()
            {
                Coordinate = new([1,1,1,1,2]),
                MemberBirthName = "Elwood Owen Vantongeren",
                MemberBirthDate = new("2013", Month.Sep),
            },
            new()
            {
                Coordinate = new([1,1,1,1,3]),
                MemberBirthName = "Debra Britny Vantongeren",
                MemberBirthDate = new("2008", Month.Apr, 28),
                MemberDeceasedDate = new("2018", Month.Feb, 8),
            },
            new()
            {
                Coordinate = new([1,1,1,1,4]),
                MemberBirthName = "Leon Adam Vantongeren",
                MemberBirthDate = new("2008"),
                MemberDeceasedDate = new("2016", Month.Feb, 21),
            },
            new()
            {
                Coordinate = new([1,1,1,1,5]),
                MemberBirthName = "Miranda Evelyn Renquist",
                MemberBirthDate = new("2018", Month.Aug, 12),
                MemberDeceasedDate = new("2026", Month.Dec, 24),
            },
            new()
            {
                Coordinate = new([1,1,1,2]),
                MemberBirthName = "Paul Keith Vantongeren",
                MemberBirthDate = new("1978", Month.Aug, 24),
                MemberDeceasedDate = new("1995", Month.Feb, 16),
                InLawBirthName = "Madilynn Samantha Marchetti",
                InLawBirthDate = new("1982"),
                FamilyDynamicStartDate = new("2013", Month.Jul, 13),
            },
            new()
            {
                Coordinate = new([1,1,1,2,1]),
                MemberBirthName = "Sharon Marcie Vantongeren",
                MemberBirthDate = new("2016"),
            },
            new()
            {
                Coordinate = new([1,1,1,2,2]),
                MemberBirthName = "Gladys \"Junior\" Taryn Vantongeren",
                MemberBirthDate = new("2006", Month.Jun, 24),
                MemberDeceasedDate = new("2017", Month.Feb, 23),
            },
            new()
            {
                Coordinate = new([1,1,1,2,3]),
                MemberBirthName = "Daniel Daniel Vantongeren",
                MemberBirthDate = new("2007", Month.Mar, 25),
                MemberDeceasedDate = new("2025", Month.Oct, 6),
            },
            new()
            {
                Coordinate = new([1,1,1,2,4]),
                MemberBirthName = "Daniel Mitchell Vantongeren",
                MemberBirthDate = new("2001", Month.Oct),
                MemberDeceasedDate = new("2026", Month.Jul, 14),
            },
            new()
            {
                Coordinate = new([1,2]),
                MemberBirthName = "Heidi Teri Vantongeren",
                MemberBirthDate = new("1952"),
            },
            new()
            {
                Coordinate = new([1,3]),
                MemberBirthName = "Kaylynn Brooke Vantongeren",
                MemberBirthDate = new("1950", Month.Oct, 19),
                MemberDeceasedDate = new("1952", Month.Nov, 20),
            },
            new()
            {
                Coordinate = new([2]),
                MemberBirthName = "Darla Malinda Vantongeren",
                MemberBirthDate = new("1898", Month.Jan, 9),
                MemberDeceasedDate = new("1936", Month.Sep, 27),
                InLawBirthName = "Holly Jamie DeWerff",
                InLawBirthDate = new("1916", Month.Dec, 9),
                InLawDeceasedDate = new("1979", Month.Nov, 9),
                FamilyDynamicStartDate = new("1939", Month.Aug, 17),
            },
            new()
            {
                Coordinate = new([2,1]),
                MemberBirthName = "Leon Zachary Marchetti",
                MemberBirthDate = new("1936", Month.Jan, 3),
                InLawBirthName = "Jason Reed Overby",
                InLawBirthDate = new("1935", Month.Dec, 14),
                InLawDeceasedDate = new("1976", Month.Sep, 4),
                FamilyDynamicStartDate = new("1957", Month.Aug, 26),
            },
            new()
            {
                Coordinate = new([2,2]),
                MemberBirthName = "Lee Ann Debra Bergstrom",
                MemberBirthDate = new("1938"),
                MemberDeceasedDate = new("1960", Month.Nov, 13),
                InLawBirthName = "Gordon Jason Kilbride",
                InLawBirthDate = new("1934", Month.Sep, 24),
                FamilyDynamicStartDate = new("1958", Month.Sep, 14),
            },
            new()
            {
                Coordinate = new([2,2,1]),
                MemberBirthName = "Jordan Leva Bergstrom",
                MemberBirthDate = new("1974", Month.Jul, 22),
                InLawBirthName = "Morten LeRoy Marchetti",
                InLawBirthDate = new("1961", Month.Dec, 25),
                InLawDeceasedDate = new("1975"),
                FamilyDynamicStartDate = new("1976", Month.Apr, 1),
            },
            new()
            {
                Coordinate = new([2,2,1,1]),
                MemberBirthName = "Hannah Shelby Bergstrom",
                MemberBirthDate = new("1991", Month.May, 21),
            },
            new()
            {
                Coordinate = new([2,2,1,2]),
                MemberBirthName = "Dorothy Eloise Bergstrom",
                MemberBirthDate = new("1991"),
            },
            new()
            {
                Coordinate = new([2,2,1,3]),
                MemberBirthName = "Keith \"Sis\" LeRoy Bergstrom",
                MemberBirthDate = new("1978"),
                InLawBirthName = "Marlin Steven Sorensen",
                InLawBirthDate = new("1980", Month.Apr, 13),
                InLawDeceasedDate = new("2013", Month.Feb, 26),
            },
            new()
            {
                Coordinate = new([2,2,1,3,1]),
                MemberBirthName = "Kory Dale Bergstrom IV",
                MemberBirthDate = new("2017", Month.May, 11),
                MemberDeceasedDate = new("2024-2026", Month.Feb),
            },
            new()
            {
                Coordinate = new([2,2,1,3,2]),
                MemberBirthName = "Joshua Gordon Bergstrom",
                MemberBirthDate = new("2014", Month.Mar),
            },
            new()
            {
                Coordinate = new([2,2,1,4]),
                MemberBirthName = "Alayna Lisa Bergstrom III",
                MemberBirthDate = new("1990", Month.Aug),
                InLawBirthName = "Teri Leva Kessler",
                InLawBirthDate = new("1983"),
                FamilyDynamicStartDate = new("2023"),
            },
            new()
            {
                Coordinate = new([2,2,1,4,1]),
                MemberBirthName = "Robert Kevin Bergstrom",
                MemberBirthDate = new("2009"),
            },
            new()
            {
                Coordinate = new([2,2,1,4,2]),
                MemberBirthName = "Wilhelm Brandon Bergstrom",
                MemberBirthDate = new("2018", Month.Apr, 27),
            },
            new()
            {
                Coordinate = new([2,2,2]),
                MemberBirthName = "Marlys Marcie Bergstrom",
                MemberBirthDate = new("1959"),
                InLawBirthName = "Lillian Sharon Overby",
                InLawBirthDate = new("1956", Month.May, 24),
                FamilyDynamicStartDate = new("1980", Month.May, 10),
            },
            new()
            {
                Coordinate = new([2,3]),
                MemberBirthName = "Conrad Jeremy Vantongeren",
                MemberBirthDate = new("1937", Month.May, 4),
            },
            new()
            {
                Coordinate = new([3]),
                MemberBirthName = "Shelby Katelyn Vantongeren",
                MemberBirthDate = new("1897", Month.Feb, 27),
                MemberDeceasedDate = new("1920", Month.May, 14),
                InLawBirthName = "Elwood Terry Sorensen",
                InLawBirthDate = new("1918"),
                InLawDeceasedDate = new("1938", Month.May, 2),
                FamilyDynamicStartDate = new("1943", Month.Sep, 16),
            },
            new()
            {
                Coordinate = new([3,1]),
                MemberBirthName = "Olivia Frieda Thornwood",
                MemberBirthDate = new("1940", Month.Dec, 20),
                MemberDeceasedDate = new("1980", Month.Nov, 6),
                InLawBirthName = "Robert Joshua Vasterling",
                InLawBirthDate = new("1936", Month.Jul, 20),
                InLawDeceasedDate = new("2002", Month.Aug, 15),
                FamilyDynamicStartDate = new("1979", Month.Apr, 5),
            },
            new()
            {
                Coordinate = new([3,1,1]),
                MemberBirthName = "Kaylynn Brian Thornwood IV",
                MemberBirthDate = new("1960", Month.Jun, 7),
                InLawBirthName = "Lillian Frieda Delacroix Sr",
                InLawBirthDate = new("1958", Month.Apr),
                FamilyDynamicStartDate = new("1980", Month.Nov),
            },
            new()
            {
                Coordinate = new([3,1,1,1]),
                MemberBirthName = "Evelyn Kinsey Thornwood",
                MemberBirthDate = new("1988", Month.Mar, 4),
                InLawBirthName = "Brooke Alison Nordstrom",
                InLawBirthDate = new("1980", Month.Dec),
                FamilyDynamicStartDate = new("2015", Month.Dec, 4),
            },
            new()
            {
                Coordinate = new([3,1,1,1,1]),
                MemberBirthName = "Hunter Ricky Thornwood",
                MemberBirthDate = new("2010", Month.Oct),
            },
            new()
            {
                Coordinate = new([3,1,1,2]),
                MemberBirthName = "Debra Ryleigh Thornwood",
                MemberBirthDate = new("1993", Month.Jun, 10),
                InLawBirthName = "Dorothy \"Dutch\" Teri Nordstrom",
                InLawBirthDate = new("1979", Month.Mar, 24),
                InLawDeceasedDate = new("2000", Month.Nov, 14),
                FamilyDynamicStartDate = new("2010", Month.Feb, 19),
            },
            new()
            {
                Coordinate = new([3,1,1,2,1]),
                MemberBirthName = "Jeremy Gordon Thornwood",
                MemberBirthDate = new("2014", Month.Mar, 22),
            },
            new()
            {
                Coordinate = new([3,1,1,2,2]),
                MemberBirthName = "Quinn Jeremy Thornwood",
                MemberBirthDate = new("2014", Month.Mar, 22),
            },
            new()
            {
                Coordinate = new([3,1,1,2,3]),
                MemberBirthName = "Laurel \"Junior\" Kinsey Thornwood",
                MemberBirthDate = new("2014", Month.Mar, 22),
            },
            new()
            {
                Coordinate = new([3,1,1,2,4]),
                MemberBirthName = "Reed Darin Thornwood",
                MemberBirthDate = new("2014", Month.Mar, 22),
            },
            new()
            {
                Coordinate = new([3,1,1,2,5]),
                MemberBirthName = "Gladys Ella Thornwood",
                MemberBirthDate = new("2014", Month.Mar, 22),
            },
            new()
            {
                Coordinate = new([3,1,1,3]),
                MemberBirthName = "Owen Todd Thornwood",
                MemberBirthDate = new("1995", Month.Mar, 15),
                InLawBirthName = "Kaylynn Paul Wachtler",
                InLawBirthDate = new("1984", Month.Mar, 5),
                InLawDeceasedDate = new("2011", Month.Jun, 6),
                FamilyDynamicStartDate = new("2013", Month.Mar),
            },
            new()
            {
                Coordinate = new([3,1,1,3,1]),
                MemberBirthName = "Hallie Joshua Thornwood",
                MemberBirthDate = new("2013", Month.May, 21),
            },
        ];

    }
}
