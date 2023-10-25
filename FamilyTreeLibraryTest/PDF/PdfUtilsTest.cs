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
        public void TestGetPDFLinesAsQueue32()
        {
            Queue<string> expected = PdfUtils.GetPDFLinesAsQueue(31, FamilyTreeUtilsTest.PDF_FILE);
            expected.Enqueue("(1) Ella Elizabeth Egeberg 01 Dec 2014 Sharon Lynn Wallace Sellie 03 Feb 1958 18 Apr 2015 Joseph Burgard 26 Apr 1960");
            Queue<string> actual = PdfUtils.GetPDFLinesAsQueue(32, FamilyTreeUtilsTest.PDF_FILE);
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
        public void TestParseAsSubNodes1()
        {
            Person member = new("August Fred Pfingsten")
            {
                BirthDate = new FamilyTreeDate("26 Jun 1896"),
                DeceasedDate = new FamilyTreeDate("24 Aug 1980")
            };
            Person inLaw = new("Frieda nee Schobinger")
            {
                BirthDate = new FamilyTreeDate("10 Nov 1902"),
                DeceasedDate = new FamilyTreeDate("13 Jul 1938")
            };
            Family family = new(member)
            {
                InLaw = inLaw,
                MarriageDate = new FamilyTreeDate("14 Sep 1921")
            };
            IReadOnlyDictionary<AbstractOrderingType,Family> expected = new Dictionary<AbstractOrderingType,Family>()
            {
                {AbstractOrderingType.GetOrderingType(1,1), family}
            };
            string[] tokens = "I. August Fred Pfingsten 26 Jun 1896 14 Sep 1921 24 Aug 1980 Frieda nee Schobinger 10 Nov 1902 13 Jul 1938".Split(' ');
            Queue<Line> lines = PdfUtils.GetLines(tokens);
            IReadOnlyDictionary<AbstractOrderingType,Family> actual = PdfUtils.ParseAsSubNodes(AbstractOrderingType.GetOrderingType(1,1), lines);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestParseAsSubNodes2()
        {
            Person member = new("Beth Marie Breyer")
            {
                BirthDate = new FamilyTreeDate("26 Feb 1976")
            };
            Person inLaw = new("Bobby Trabue")
            {
                BirthDate = new FamilyTreeDate("07 Jun 1979")
            };
            Family fam = new(member)
            {
                InLaw = inLaw,
                MarriageDate = new FamilyTreeDate("Mar 2002")
            };
            IReadOnlyDictionary<AbstractOrderingType,Family> expected = new Dictionary<AbstractOrderingType,Family>()
            {
                {AbstractOrderingType.GetOrderingType(1, 4), fam}
            };
            string[] tokens = "a. Beth Marie Breyer 26 Feb 1976 Mar 2002 Bobby Trabue 07 Jun 1979".Split(' ');
            Queue<Line> lines = PdfUtils.GetLines(tokens);
            IReadOnlyDictionary<AbstractOrderingType,Family> actual = PdfUtils.ParseAsSubNodes(AbstractOrderingType.GetOrderingType(1,4), lines);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestParseAsFamilyNodesWith1()
        {
            Person member = new("August Fred Pfingsten")
            {
                BirthDate = new FamilyTreeDate("26 Jun 1896"),
                DeceasedDate = new FamilyTreeDate("24 Aug 1980")
            };
            Person inLaw = new("Frieda nee Schobinger")
            {
                BirthDate = new FamilyTreeDate("10 Nov 1902"),
                DeceasedDate = new FamilyTreeDate("13 Jul 1938")
            };
            Family family = new(member)
            {
                InLaw = inLaw,
                MarriageDate = new FamilyTreeDate("14 Sep 1921")
            };
            KeyValuePair<int,Family> expectedPair = new(0, family);
            Queue<KeyValuePair<int,Family>> expectedQueue = new();
            expectedQueue.Enqueue(expectedPair);
            IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> expected = new Dictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>>()
            {
                {new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1)}, expectedQueue}
            };
            Queue<string> textLines = PdfUtils.GetPDFLinesAsQueue(1, FamilyTreeUtilsTest.PDF_FILE);
            IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> actual = PdfUtils.ParseAsFamilyNodes(textLines);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestParseAsFamilyNodesWith2()
        {
            Person member = new("August Fred Pfingsten")
            {
                BirthDate = new FamilyTreeDate("26 Jun 1896"),
                DeceasedDate = new FamilyTreeDate("24 Aug 1980")
            };
            Person inLaw = new("Frieda nee Schobinger")
            {
                BirthDate = new FamilyTreeDate("10 Nov 1902"),
                DeceasedDate = new FamilyTreeDate("13 Jul 1938")
            };
            Family family1 = new(member)
            {
                InLaw = inLaw,
                MarriageDate = new FamilyTreeDate("14 Sep 1921")
            };
            KeyValuePair<int,Family> expectedPair1 = new(0, family1);
            Queue<KeyValuePair<int,Family>> expectedQueue1 = new();
            expectedQueue1.Enqueue(expectedPair1);
            Person member2 = new("Lillian Pfingsten")
            {
                BirthDate = new FamilyTreeDate("12 Nov 1922"),
                DeceasedDate = new FamilyTreeDate("9 Oct 1924")
            };
            Family family2 = new(member2);
            KeyValuePair<int,Family> expectedPair2 = new(1, family2);
            Queue<KeyValuePair<int,Family>> expectedQueue2 = new();
            expectedQueue2.Enqueue(expectedPair2);
            IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> expected = new Dictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>>()
            {
                {new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1)}, expectedQueue1},
                {new AbstractOrderingType[] {AbstractOrderingType.GetOrderingType(1,1), AbstractOrderingType.GetOrderingType(1,2)}, expectedQueue2}
            };
            Queue<string> textLines = PdfUtils.GetPDFLinesAsQueue(2, FamilyTreeUtilsTest.PDF_FILE);
            IReadOnlyDictionary<AbstractOrderingType[],Queue<KeyValuePair<int,Family>>> actual = PdfUtils.ParseAsFamilyNodes(textLines);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestParseAsFamilyNodesWith7()
        {
            Person member7 = new("Evelyn Pfingsten")
            {
                BirthDate = new FamilyTreeDate("14 Mar 1926")
            };
            Person inLaw7 = new("Gordon Fred Kuder")
            {
                BirthDate = new FamilyTreeDate("07 Mar 1924"),
                DeceasedDate = new FamilyTreeDate("11 Dec 2014")
            };
            Family family7 = new(member7)
            {
                InLaw = inLaw7,
                MarriageDate = new FamilyTreeDate("07 Aug 1947")
            };
            KeyValuePair<int,Family> pair7 = new(6, family7);
            Queue<KeyValuePair<int,Family>> queue7 = new();
            queue7.Enqueue(pair7);
            AbstractOrderingType[] key7 = new AbstractOrderingType[]
            {
                AbstractOrderingType.GetOrderingType(1,1),
                AbstractOrderingType.GetOrderingType(3,2)
            };
            Queue<KeyValuePair<int,Family>> actualQueue7 = PdfUtils.ParseAsFamilyNodes(PdfUtils.GetPDFLinesAsQueue(7, FamilyTreeUtilsTest.PDF_FILE))[key7];
            Assert.That(actualQueue7, Is.EqualTo(queue7));
        }

        [Test]
        public void TestParseAsFamilyNodesWith11()
        {
            Person member11 = new("Gladys Lydia Pfingsten")
            {
                BirthDate = new FamilyTreeDate("03 Jan 1931"),
                DeceasedDate = new FamilyTreeDate("12 Apr 2004")
            };
            Person inLaw11 = new("Owen Clayton Wallace")
            {
                BirthDate = new FamilyTreeDate("09 Apr 1928"),
                DeceasedDate = new FamilyTreeDate("31 May 2017")
            };
            Family family11 = new(member11)
            {
                InLaw = inLaw11,
                MarriageDate = new FamilyTreeDate("20 Jan 1951")
            };
            KeyValuePair<int,Family> pair11 = new(10, family11);
            Queue<KeyValuePair<int,Family>> queue11 = new();
            queue11.Enqueue(pair11);
            AbstractOrderingType[] key11 = new AbstractOrderingType[]
            {
                AbstractOrderingType.GetOrderingType(1,1),
                AbstractOrderingType.GetOrderingType(5,2)
            };
            Queue<KeyValuePair<int,Family>> actualQueue11 = PdfUtils.ParseAsFamilyNodes(PdfUtils.GetPDFLinesAsQueue(11, FamilyTreeUtilsTest.PDF_FILE))[key11];
            Assert.That(actualQueue11, Is.EqualTo(queue11));
        }

        [Test]
        public void TestParseAsFamilyNodesWith13()
        {
            Person member13 = new("Beth Marie Breyer")
            {
                BirthDate = new FamilyTreeDate("20 Jan 1951"),
                DeceasedDate = new FamilyTreeDate("26 Feb 1976")
            };
            Person inLaw13 = new("Bobby Trabue")
            {
                BirthDate = new FamilyTreeDate("26 Feb 1976"),
                DeceasedDate = new FamilyTreeDate("07 Jun 1979")
            };
            Family fam = new(member13)
            {
                InLaw = inLaw13,
                MarriageDate = new FamilyTreeDate("01 Mar 2002")
            };
            KeyValuePair<int,Family> pair13 = new(12,fam);
            Queue<KeyValuePair<int,Family>> queue13 = new();
            queue13.Enqueue(pair13);
            AbstractOrderingType[] key13 = new AbstractOrderingType[]
            {
                AbstractOrderingType.GetOrderingType(1,1),
                AbstractOrderingType.GetOrderingType(5,2),
                AbstractOrderingType.GetOrderingType(1,3),
                AbstractOrderingType.GetOrderingType(1,4)
            };
            Queue<KeyValuePair<int,Family>> actualQueue13 = PdfUtils.ParseAsFamilyNodes(PdfUtils.GetPDFLinesAsQueue(13, FamilyTreeUtilsTest.PDF_FILE))[key13];
            Assert.That(actualQueue13, Is.EqualTo(queue13));
        }

        [Test]
        public void TestParseAsFamilyNodesWithAdditionalNameInfomationOnNextLine()
        {
            Person member21 = new("Taryn Elizabeth Allmaras")
            {
                BirthDate = new FamilyTreeDate("01 Mar 2002"),
                DeceasedDate = new FamilyTreeDate("04 Apr 2001")
            };
            Family family21 = new(member21);
            KeyValuePair<int,Family> pair21 = new(20, family21);
            Queue<KeyValuePair<int,Family>> queue21 = new();
            queue21.Enqueue(pair21);
            AbstractOrderingType[] key21 = new AbstractOrderingType[]
            {
                AbstractOrderingType.GetOrderingType(1,1),
                AbstractOrderingType.GetOrderingType(5,2),
                AbstractOrderingType.GetOrderingType(2,3),
                AbstractOrderingType.GetOrderingType(1,4),
                AbstractOrderingType.GetOrderingType(1,5)
            };
            Queue<KeyValuePair<int,Family>> actualQueue21 = PdfUtils.ParseAsFamilyNodes(PdfUtils.GetPDFLinesAsQueue(21, FamilyTreeUtilsTest.PDF_FILE))[key21];
            Assert.That(actualQueue21, Is.EqualTo(queue21));
        }
    }
}