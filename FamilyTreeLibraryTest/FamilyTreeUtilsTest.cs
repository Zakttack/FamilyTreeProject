using FamilyTreeLibrary;
using FamilyTreeLibrary.OrderingType;

namespace FamilyTreeLibraryTest
{
    public class FamilyTreeUtilsTest
    {
        public const string PDF_FILE = @"C:\Users\zakme\Documents\FamilyTreeProject\Resources\PfingstenBook2023.pdf";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetFileNameFromResources()
        {
            string actaul = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), "PfingstenBook2023.pdf");
            Assert.That(actaul, Is.EqualTo(PDF_FILE));
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
    }
}