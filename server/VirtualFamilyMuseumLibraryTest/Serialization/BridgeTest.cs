using VirtualFamilyMuseumLibrary.Serialization;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibraryTest.Serialization
{
    [TestFixture]
    public class BridgeTest
    {
        // =====================================================================
        // Value
        // =====================================================================

        [Test]
        public void Value_Null_ReturnsNullBridgeValue()
        {
            Bridge bridge = new(new BridgeValue());
            Assert.That(bridge.Value.IsNull, Is.True);
        }

        [Test]
        public void Value_String_ReturnsMatchingBridgeValue()
        {
            Bridge bridge = new("hello");
            Assert.That(bridge.Value, Is.EqualTo((BridgeValue)"hello"));
        }

        [Test]
        public void Value_Number_ReturnsMatchingBridgeValue()
        {
            Bridge bridge = new((Number)5.0);
            Assert.That(bridge.Value, Is.EqualTo((BridgeValue)(Number)5.0));
        }

        [Test]
        public void Value_Bool_ReturnsMatchingBridgeValue()
        {
            Bridge bridge = new(true);
            Assert.That(bridge.Value, Is.EqualTo((BridgeValue)true));
        }

        // =====================================================================
        // Equals
        // =====================================================================

        [Test]
        public void Equals_SameValue_ReturnsTrue()
        {
            Bridge a = new("hello");
            Bridge b = new("hello");
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            Bridge a = new("hello");
            Bridge b = new("world");
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_BothNullValues_ReturnsTrue()
        {
            Bridge a = new(new BridgeValue());
            Bridge b = new(new BridgeValue());
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_SameReference_ReturnsTrue()
        {
            Bridge a = new("hello");
            Bridge b = a;
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_NonBridgeObject_ReturnsFalse()
        {
            Bridge a = new("hello");
            Assert.That(a.Equals((object?)"hello"), Is.False);
        }

        [Test]
        public void Equals_DifferentIBridgeImplementation_ReturnsFalse()
        {
            Bridge a = new("hello");
            IBridge other = new OtherBridge((BridgeValue)"hello");
            Assert.That(a.Equals(other), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            Bridge a = new("hello");
            Assert.That(a.Equals((object?)null), Is.False);
        }

        // =====================================================================
        // GetHashCode
        // =====================================================================

        [Test]
        public void GetHashCode_EqualValues_ReturnSameHash()
        {
            Bridge a = new("hello");
            Bridge b = new("hello");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_DifferentValues_ReturnDifferentHash()
        {
            Bridge a = new("hello");
            Bridge b = new("world");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_MatchesUnderlyingValueHash()
        {
            Bridge bridge = new("hello");
            Assert.That(bridge.GetHashCode(), Is.EqualTo(((BridgeValue)"hello").GetHashCode()));
        }

        // =====================================================================
        // ToString
        // =====================================================================

        [Test]
        public void ToString_Null_ReturnsJsonNull()
        {
            Bridge bridge = new(new BridgeValue());
            Assert.That(bridge.ToString(), Is.EqualTo("null"));
        }

        [Test]
        public void ToString_String_ReturnsQuotedJsonString()
        {
            Bridge bridge = new("hello");
            Assert.That(bridge.ToString(), Is.EqualTo("\"hello\""));
        }

        [Test]
        public void ToString_Bool_ReturnsJsonBool()
        {
            Bridge bridge = new(true);
            Assert.That(bridge.ToString(), Is.EqualTo("true"));
        }

        // =====================================================================
        // Operator ==
        // =====================================================================

        [Test]
        public void OperatorEqual_BothNullReferences_ReturnsTrue()
        {
            Bridge? a = null;
            Bridge? b = null;
            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqual_LeftNullReference_ReturnsFalse()
        {
            Bridge? a = null;
            Bridge b = new("hello");
            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorEqual_RightNullReference_ReturnsFalse()
        {
            Bridge a = new("hello");
            Bridge? b = null;
            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorEqual_SameValues_ReturnsTrue()
        {
            Bridge a = new("hello");
            Bridge b = new("hello");
            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqual_DifferentValues_ReturnsFalse()
        {
            Bridge a = new("hello");
            Bridge b = new("world");
            Assert.That(a == b, Is.False);
        }

        // =====================================================================
        // Operator !=
        // =====================================================================

        [Test]
        public void OperatorNotEqual_BothNullReferences_ReturnsFalse()
        {
            Bridge? a = null;
            Bridge? b = null;
            Assert.That(a != b, Is.False);
        }

        [Test]
        public void OperatorNotEqual_LeftNullReference_ReturnsTrue()
        {
            Bridge? a = null;
            Bridge b = new("hello");
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorNotEqual_RightNullReference_ReturnsTrue()
        {
            Bridge a = new("hello");
            Bridge? b = null;
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorNotEqual_DifferentValues_ReturnsTrue()
        {
            Bridge a = new("hello");
            Bridge b = new("world");
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorNotEqual_SameValues_ReturnsFalse()
        {
            Bridge a = new("hello");
            Bridge b = new("hello");
            Assert.That(a != b, Is.False);
        }

        private sealed class OtherBridge(BridgeValue value) : IBridge
        {
            public BridgeValue Value => value;
        }
    }
}
