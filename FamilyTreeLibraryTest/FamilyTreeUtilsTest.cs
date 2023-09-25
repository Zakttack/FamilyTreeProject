using FamilyTreeLibrary;
namespace FamilyTreeLibraryTest
{
    public class FamilyTreeUtilsTest
    {

        [Test]
        public void TestGetSubCollection1()
        {
            string[] collection = new string[] {"1", "2", "3", "4", "5"};
            int start = 1;
            int end = 3;
            string[] expected = new string[] {"2", "3", "4"};
            string[] actual = FamilyTreeUtils.GetSubCollection(collection, start, end);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetSubCollection2()
        {
            string[] collection = new string[] {"1", "2", "3", "4", "5"};
            int start = 1;
            string[] expected = new string[] {"2", "3", "4", "5"};
            string[] actual = FamilyTreeUtils.GetSubCollection(collection, start);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}