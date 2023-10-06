using FamilyTreeLibrary.OrderingType;

namespace FamilyTreeLibraryTest.OrderingType
{
    public class AbstractOrderingTypeTest
    {
        [Test]
        public void TestGetOrderingType1()
        {
            int key = 1;
            int generation = 1;
            KeyValuePair<int,string> expected = new(1,"I.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType2()
        {
            int key = 2;
            int generation = 1;
            KeyValuePair<int,string> expected = new(2,"II.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType3()
        {
            int key = 1;
            int generation = 2;
            KeyValuePair<int,string> expected = new(1,"A.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType4()
        {
            int key = 2;
            int generation = 2;
            KeyValuePair<int,string> expected = new(2,"B.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType5()
        {
            int key = 1;
            int generation = 3;
            KeyValuePair<int,string> expected = new(1,"1.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType6()
        {
            int key = 2;
            int generation = 3;
            KeyValuePair<int,string> expected = new(2,"2.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType7()
        {
            int key = 1;
            int generation = 4;
            KeyValuePair<int,string> expected = new(1,"a.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType8()
        {
            int key = 2;
            int generation = 4;
            KeyValuePair<int,string> expected = new(2,"b.");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType9()
        {
            int key = 1;
            int generation = 5;
            KeyValuePair<int,string> expected = new(1,"(1)");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType10()
        {
            int key = 2;
            int generation = 5;
            KeyValuePair<int,string> expected = new(2,"(2)");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType11()
        {
            int key = 1;
            int generation = 6;
            KeyValuePair<int,string> expected = new(1,"i)");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestGetOrderingType12()
        {
            int key = 2;
            int generation = 6;
            KeyValuePair<int,string> expected = new(2,"ii)");
            AbstractOrderingType orderingType = AbstractOrderingType.GetOrderingType(key, generation);
            Assert.That(orderingType.ConversionPair, Is.EqualTo(expected));
        }

        [Test]
        public void TestTryGetOrderingType1()
        {
            string value = "I.";
            int generation = 1;
            Assert.That(AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, value, generation));
        }

        [Test]
        public void TestTryGetOrderingType2()
        {
            string value = "August";
            int generation = 2;
            Assert.False(AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, value, generation));
        }

        [Test]
        public void TestTryGetOrderingType3()
        {
            string value = "A";
            int generation = 2;
            Assert.False(AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, value, generation));
        }

    }
}