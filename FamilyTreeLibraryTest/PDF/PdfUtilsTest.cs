using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF;
using FamilyTreeLibrary.PDF.Models;

namespace FamilyTreeLibraryTest.PDF
{
    public class PdfUtilsTest
    {

        [Test]
        public void TestGetPDFLinesAsQueue1()
        {
            Queue<string> expected = new();
            expected.Enqueue("I. August Fred Pfingsten 26 Jun 1896 14 Sep 1921 24 Aug 1980 Frieda nee Schobinger 10 Nov 1902 13 Jul 1938");
            Queue<string> actual = PdfUtils.GetPDFLinesAsQueue(1, FamilyTreeUtilsTest.PDF_FILE);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetLines1()
        {
            string[] tokens = "I. August Fred Pfingsten 26 Jun 1896 14 Sep 1921 24 Aug 1980 Frieda nee Schobinger 10 Nov 1902 13 Jul 1938".Split(' ');
            Queue<Line> expected = new();
            Line expectedMember = new()
            {
                Name = "August Fred Pfingsten"
            };
            expectedMember.Dates.Enqueue(Convert.ToDateTime("26 Jun 1896"));
            expectedMember.Dates.Enqueue(Convert.ToDateTime("14 Sep 1921"));
            expectedMember.Dates.Enqueue(Convert.ToDateTime("24 Aug 1980"));
            expected.Enqueue(expectedMember);
            Line expectedInLaw = new()
            {
                Name = "Frieda nee Schobinger"
            };
            expectedInLaw.Dates.Enqueue(Convert.ToDateTime("10 Nov 1902"));
            expectedInLaw.Dates.Enqueue(Convert.ToDateTime("13 Jul 1938"));
            expected.Enqueue(expectedInLaw);
            Queue<Line> actual = PdfUtils.GetLines(tokens);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestParseAsSubNodes1()
        {
            Person member = new("August Fred Pfingsten", Convert.ToDateTime("26 Jun 1896"), Convert.ToDateTime("24 Aug 1980"));
            Person inLaw = new("Frieda nee Schobinger", Convert.ToDateTime("10 Nov 1902"), Convert.ToDateTime("13 Jul 1938"));
            Family family = new(member, inLaw, Convert.ToDateTime("14 Sep 1921"));
            IReadOnlyDictionary<AbstractOrderingType,Family> expected = new Dictionary<AbstractOrderingType,Family>()
            {
                {AbstractOrderingType.GetOrderingType(1,1), family}
            };
            string[] tokens = "I. August Fred Pfingsten 26 Jun 1896 14 Sep 1921 24 Aug 1980 Frieda nee Schobinger 10 Nov 1902 13 Jul 1938".Split(' ');
            Queue<Line> lines = PdfUtils.GetLines(tokens);
            IReadOnlyDictionary<AbstractOrderingType,Family> actual = PdfUtils.ParseAsSubNodes(AbstractOrderingType.GetOrderingType(1,1), lines);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}