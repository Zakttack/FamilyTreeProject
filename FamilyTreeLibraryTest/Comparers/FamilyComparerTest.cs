using FamilyTreeLibrary.Comparers;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibraryTest.Comparers
{
    public class FamilyComparerTest
    {
        private IComparer<Family> comparer;
        [SetUp]
        public void Setup()
        {
            comparer = new FamilyComparer();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}