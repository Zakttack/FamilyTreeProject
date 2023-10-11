using FamilyTreeLibrary;

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
        public void TestGetOrderingTypeByLine()
        {

        }
    }
}