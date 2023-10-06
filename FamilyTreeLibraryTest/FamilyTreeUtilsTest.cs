using FamilyTreeLibrary;

namespace FamilyTreeLibraryTest
{
    public class FamilyTreeUtilsTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetFileNameFromResources()
        {
            string expected = @"C:\Users\zakme\Documents\FamilyTreeProject\Resources\PfingstenBook2023.pdf";
            string actaul = FamilyTreeUtils.GetFileNameFromResources(Directory.GetCurrentDirectory(), "PfingstenBook2023.pdf");
            Assert.That(actaul, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingTypeByLine()
        {

        }
    }
}