using FamilyTreeLibrary.Comparers;

namespace FamilyTreeLibraryTest.Comparers
{
    public class DateComparerTest
    {
        private IComparer<DateTime> comparer;
        [SetUp]
        public void Setup()
        {
            comparer = new DateComparer();
        }

        [Test]
        public void TestDateTimeWithBothDefault()
        {
            DateTime a = default;
            DateTime b = default;
            int expected = 0;
            int actaul = comparer.Compare(a,b);
            Assert.That(actaul, Is.EqualTo(expected));
        }

        [Test]
        public void TestDateTimeWithFirstDefault()
        {
            DateTime a = default;
            DateTime b = new(1896, 6, 26);
            Assert.That(comparer.Compare(a,b) < 0);
        }

        [Test]
        public void TestDateTimeWithSecondDefault()
        {
            DateTime a = new(1896, 6, 26);
            DateTime b = default;
            Assert.That(comparer.Compare(a,b) > 0);
        }
    }
}