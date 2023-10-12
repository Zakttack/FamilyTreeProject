using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using FamilyTreeLibrary.PDF;
namespace FamilyTreeLibraryTest.PDF
{
    public class PdfClientTest
    {

        [Test]
        public void TestWith1()
        {
            Person member = new("August Fred Pfingsten", Convert.ToDateTime("26 Jun 1896"), Convert.ToDateTime("24 Aug 1980"));
            Person inLaw = new("Frieda nee Schobinger", Convert.ToDateTime("10 Nov 1902"), Convert.ToDateTime("13 Jul 1938"));
            Family family = new(member, inLaw, Convert.ToDateTime("14 Sep 1921"));
            KeyValuePair<int,Family> expectedPair = new(0, family);
            Queue<KeyValuePair<int,Family>> expectedQueue = new();
            expectedQueue.Enqueue(expectedPair);
            IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> expected = new Dictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>>()
            {
                {new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1)}, expectedQueue}
            };
            PdfClient client = new(FamilyTreeUtilsTest.PDF_FILE, 1);
            client.LoadNodes();
            Assert.That(client.Nodes, Is.EqualTo(expected));
        }

        [Test]
        public void TestWith2()
        {
            Person member1 = new("August Fred Pfingsten", Convert.ToDateTime("26 Jun 1896"), Convert.ToDateTime("24 Aug 1980"));
            Person inLaw1 = new("Frieda nee Schobinger", Convert.ToDateTime("10 Nov 1902"), Convert.ToDateTime("13 Jul 1938"));
            Family family1 = new(member1, inLaw1, Convert.ToDateTime("14 Sep 1921"));
            KeyValuePair<int,Family> expectedPair1 = new(0, family1);
            Queue<KeyValuePair<int,Family>> expectedQueue1 = new();
            expectedQueue1.Enqueue(expectedPair1);
            Person member2 = new("Lillian Pfingsten", Convert.ToDateTime("12 Nov 1922"), Convert.ToDateTime("9 Oct 1924"));
            Family family2 = new(member2);
            KeyValuePair<int,Family> expectedPair2 = new(1, family2);
            Queue<KeyValuePair<int,Family>> expectedQueue2 = new();
            expectedQueue2.Enqueue(expectedPair2);
            IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> expected = new Dictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>>()
            {
                {new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1)}, expectedQueue1},
                {new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1), AbstractOrderingType.GetOrderingType(1,2)}, expectedQueue2}
            };
            PdfClient client = new(FamilyTreeUtilsTest.PDF_FILE, 2);
            client.LoadNodes();
            Assert.That(client.Nodes, Is.EqualTo(expected));
        }
    }
}