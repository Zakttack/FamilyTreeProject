using FamilyTreeLibrary.PDF.Models;

namespace FamilyTreeLibraryTest.PDF.Models
{
    public class LineTest
    {

        [Test]
        public void TestCopy()
        {
            Line expected = new()
            {
                Name = "August Fred Pfingsten"
            };
            expected.Dates.Enqueue(Convert.ToDateTime("26 Jun 1896"));
            expected.Dates.Enqueue(Convert.ToDateTime("14 Sep 1921"));
            expected.Dates.Enqueue(Convert.ToDateTime("24 Aug 1980"));
            Assert.That(expected.Copy(), Is.EqualTo(expected));
        }
    }
}