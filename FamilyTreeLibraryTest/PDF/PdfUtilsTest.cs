using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF;
using FamilyTreeLibrary.PDF.Models;

namespace FamilyTreeLibraryTest.PDF
{
    public class PdfUtilsTest
    {

        [Test]
        public void TestGetLines1()
        {
            string[] tokens = "I. August Fred Pfingsten 26 Jun 1896 14 Sep 1921 24 Aug 1980 Frieda nee Schobinger 10 Nov 1902 13 Jul 1938".Split(' ');
            Queue<Line> expected = new();
            Line expectedMember = new()
            {
                Name = "August Fred Pfingsten"
            };
            expectedMember.Dates.Enqueue(new FamilyTreeDate("26 Jun 1896"));
            expectedMember.Dates.Enqueue(new FamilyTreeDate("14 Sep 1921"));
            expectedMember.Dates.Enqueue(new FamilyTreeDate("24 Aug 1980"));
            expected.Enqueue(expectedMember);
            Line expectedInLaw = new()
            {
                Name = "Frieda nee Schobinger"
            };
            expectedInLaw.Dates.Enqueue(new FamilyTreeDate("10 Nov 1902"));
            expectedInLaw.Dates.Enqueue(new FamilyTreeDate("13 Jul 1938"));
            expected.Enqueue(expectedInLaw);
            Queue<Line> actual = PdfUtils.GetLines(tokens);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestOrderingTypeArrayEquals1()
        {
            AbstractOrderingType[] orderingType1 = {AbstractOrderingType.GetOrderingType(1,1), AbstractOrderingType.GetOrderingType(5,2), AbstractOrderingType.GetOrderingType(3,3)};
            AbstractOrderingType[] orderingType2 = {AbstractOrderingType.GetOrderingType(1,1), AbstractOrderingType.GetOrderingType(5,2), AbstractOrderingType.GetOrderingType(3,3)};
            Assert.That(orderingType1, Is.EqualTo(orderingType2));
        }
    }
}