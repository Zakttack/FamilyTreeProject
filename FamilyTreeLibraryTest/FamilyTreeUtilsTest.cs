using FamilyTreeLibrary;
using FamilyTreeLibrary.OrderingType;

namespace FamilyTreeLibraryTest
{
    public class FamilyTreeUtilsTest
    {
        internal const string PDF_FILE = @"C:\Users\zakme\Documents\FamilyTreeProject\Resources\PfingstenBook2023.pdf";

        [Test]
        public void TestGateDate1()
        {
            DateTime expected = Convert.ToDateTime("01 Mar 2002");
            DateTime actual = FamilyTreeUtils.GetDate("Mar 2002");
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetFileNameFromResources()
        {
            string actaul = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), "PfingstenBook2023.pdf");
            Assert.That(actaul, Is.EqualTo(PDF_FILE));
        }

        [Test]
        public void TestGetOrderingTypeByLine1()
        {
            Queue<AbstractOrderingType> expectedOrderingTypePossibilities = new();
            expectedOrderingTypePossibilities.Enqueue(AbstractOrderingType.GetOrderingType(100, 1));
            expectedOrderingTypePossibilities.Enqueue(AbstractOrderingType.GetOrderingType(3,2));
            string line = "C. Evelyn Pfingsten 14 Mar 1926 07 Aug 1947";
            Queue<AbstractOrderingType> actualOrderingTypePossibilities = FamilyTreeUtils.GetOrderingTypeByLine(line);
            Assert.That(actualOrderingTypePossibilities, Is.EqualTo(expectedOrderingTypePossibilities));
        }

        [Test]
        public void TestNextOrderingType1()
        {
            AbstractOrderingType[] expected = new AbstractOrderingType[]{AbstractOrderingType.GetOrderingType(1,1)};
            AbstractOrderingType[] actual = FamilyTreeUtils.NextOrderingType(new AbstractOrderingType[0], AbstractOrderingType.GetOrderingType(1,1));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestNextOrderingType2()
        {
            AbstractOrderingType[] expected = new AbstractOrderingType[]{AbstractOrderingType.GetOrderingType(1,1), AbstractOrderingType.GetOrderingType(1,2)};
            AbstractOrderingType[] current = new AbstractOrderingType[]{AbstractOrderingType.GetOrderingType(1,1)};
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(1,2);
            Assert.That(FamilyTreeUtils.NextOrderingType(current, orderingType), Is.EqualTo(expected));
        }

        [Test]
        public void TestNextOrderingType3()
        {
            AbstractOrderingType[] expected = new AbstractOrderingType[]
            {
                AbstractOrderingType.GetOrderingType(1,1),
                AbstractOrderingType.GetOrderingType(3,2)
            };
            AbstractOrderingType[] current = new AbstractOrderingType[]
            {
                AbstractOrderingType.GetOrderingType(1,1),
                AbstractOrderingType.GetOrderingType(2,2),
                AbstractOrderingType.GetOrderingType(2,3),
                AbstractOrderingType.GetOrderingType(1,4)
            };
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(3,2);
            Assert.That(FamilyTreeUtils.NextOrderingType(current, orderingType), Is.EqualTo(expected));
        }

        [Test]
        public void TestPreviousOrderingType1()
        {
            AbstractOrderingType[] expected = new AbstractOrderingType[0];
            AbstractOrderingType[] current = new AbstractOrderingType[]{AbstractOrderingType.GetOrderingType(1,1)};
            AbstractOrderingType[] actual = FamilyTreeUtils.PreviousOrderingType(current);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestReformatToken1()
        {
            string expected = "31 May 2017";
            string actual = FamilyTreeUtils.ReformatToken("31May2017");
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}