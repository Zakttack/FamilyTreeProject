using System.Text.Json;
using VirtualFamilyMuseumLibrary.Models;
using VirtualFamilyMuseumLibrary.Serialization;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibraryTest.Serialization
{
    [TestFixture]
    public class SerializationExtensionsTest
    {
        // =====================================================================
        // GetOptions
        // =====================================================================

        [Test]
        public void GetOptions_WriteIndentedTrue_SetsWriteIndented()
        {
            JsonSerializerOptions options = SerializationExtensions.GetOptions(true);
            Assert.That(options.WriteIndented, Is.True);
        }

        [Test]
        public void GetOptions_WriteIndentedFalse_SetsWriteIndented()
        {
            JsonSerializerOptions options = SerializationExtensions.GetOptions(false);
            Assert.That(options.WriteIndented, Is.False);
        }

        [Test]
        public void GetOptions_Always_RegistersBridgeSerializer()
        {
            JsonSerializerOptions options = SerializationExtensions.GetOptions(true);
            Assert.That(options.Converters, Has.Count.EqualTo(1));
            Assert.That(options.Converters[0], Is.TypeOf<BridgeSerializer>());
        }

        // =====================================================================
        // DeserializeNull / SerializeNull
        // =====================================================================

        [Test]
        public void DeserializeNull_NullBridge_ReturnsNull()
        {
            IBridge bridge = new Bridge(new BridgeValue());
            Assert.That(bridge.DeserializeNull(), Is.Null);
        }

        [Test]
        public void DeserializeNull_NonNullBridge_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge("hello");
            Assert.That(() => bridge.DeserializeNull(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SerializeNull_Always_ReturnsBridgeWithNullValue()
        {
            IBridge bridge = new Bridge("hello");
            IBridge result = bridge.SerializeNull();
            Assert.That(result.Value.IsNull, Is.True);
        }

        // =====================================================================
        // DeserializeString / SerializeString
        // =====================================================================

        [Test]
        public void DeserializeString_StringBridge_ReturnsValue()
        {
            IBridge bridge = new Bridge("hello");
            Assert.That(bridge.DeserializeString(), Is.EqualTo("hello"));
        }

        [Test]
        public void DeserializeString_WrongType_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge(true);
            Assert.That(() => bridge.DeserializeString(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SerializeString_Always_ReturnsBridgeWithMatchingValue()
        {
            IBridge bridge = new Bridge(new BridgeValue());
            IBridge result = bridge.SerializeString("hello");
            Assert.That(result.Value, Is.EqualTo((BridgeValue)"hello"));
        }

        // =====================================================================
        // DeserializeInt / SerializeInt
        // =====================================================================

        [Test]
        public void DeserializeInt_WholeNumberBridge_ReturnsValue()
        {
            IBridge bridge = new Bridge((Number)42.0);
            Assert.That(bridge.DeserializeInt(), Is.EqualTo(42));
        }

        [Test]
        public void DeserializeInt_DecimalNumberBridge_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge((Number)3.14);
            Assert.That(() => bridge.DeserializeInt(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void DeserializeInt_WrongType_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge("42");
            Assert.That(() => bridge.DeserializeInt(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SerializeInt_Always_ReturnsBridgeWithMatchingValue()
        {
            IBridge bridge = new Bridge(new BridgeValue());
            IBridge result = bridge.SerializeInt(42);
            Assert.That(result.Value, Is.EqualTo((BridgeValue)(Number)42.0));
        }

        // =====================================================================
        // DeserializeLong / SerializeLong
        // =====================================================================

        [Test]
        public void DeserializeLong_WholeNumberBridge_ReturnsValue()
        {
            IBridge bridge = new Bridge((Number)123456789L);
            Assert.That(bridge.DeserializeLong(), Is.EqualTo(123456789L));
        }

        [Test]
        public void DeserializeLong_DecimalNumberBridge_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge((Number)9.9);
            Assert.That(() => bridge.DeserializeLong(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void DeserializeLong_WrongType_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge("100");
            Assert.That(() => bridge.DeserializeLong(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SerializeLong_Always_ReturnsBridgeWithMatchingValue()
        {
            IBridge bridge = new Bridge(new BridgeValue());
            IBridge result = bridge.SerializeLong(123456789L);
            Assert.That(result.Value, Is.EqualTo((BridgeValue)(Number)123456789L));
        }

        // =====================================================================
        // DeserializeDouble / SerializeDouble
        // =====================================================================

        [Test]
        public void DeserializeDouble_NumberBridge_ReturnsValue()
        {
            IBridge bridge = new Bridge((Number)3.14);
            Assert.That(bridge.DeserializeDouble(), Is.EqualTo(3.14));
        }

        [Test]
        public void DeserializeDouble_WrongType_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge("3.14");
            Assert.That(() => bridge.DeserializeDouble(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SerializeDouble_Always_ReturnsBridgeWithMatchingValue()
        {
            IBridge bridge = new Bridge(new BridgeValue());
            IBridge result = bridge.SerializeDouble(3.14);
            Assert.That(result.Value, Is.EqualTo((BridgeValue)(Number)3.14));
        }

        // =====================================================================
        // DeserializeObject / SerializeObject
        // =====================================================================

        [Test]
        public void DeserializeObject_ObjectBridge_ReturnsMatchingEntries()
        {
            IDictionary<string, BridgeValue> source = new Dictionary<string, BridgeValue> { ["a"] = "1" };
            IBridge bridge = new Bridge(new BridgeValue(source));
            Assert.That(bridge.DeserializeObject()["a"], Is.EqualTo((BridgeValue)"1"));
        }

        [Test]
        public void DeserializeObject_WrongType_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge("hello");
            Assert.That(() => bridge.DeserializeObject(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SerializeObject_Always_ReturnsBridgeWithMatchingValue()
        {
            IDictionary<string, BridgeValue> source = new Dictionary<string, BridgeValue> { ["a"] = "1" };
            IBridge bridge = new Bridge(new BridgeValue());
            IBridge result = bridge.SerializeObject(source);
            Assert.That(result.Value, Is.EqualTo(new BridgeValue(source)));
        }

        // =====================================================================
        // DeserializeArray / SerializeArray
        // =====================================================================

        [Test]
        public void DeserializeArray_ArrayBridge_ReturnsMatchingElements()
        {
            IList<BridgeValue> source = ["1", "2"];
            IBridge bridge = new Bridge(new BridgeValue(source));
            Assert.That(bridge.DeserializeArray(), Is.EqualTo(source));
        }

        [Test]
        public void DeserializeArray_WrongType_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge("hello");
            Assert.That(() => bridge.DeserializeArray(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SerializeArray_Always_ReturnsBridgeWithMatchingValue()
        {
            IList<BridgeValue> source = ["1", "2"];
            IBridge bridge = new Bridge(new BridgeValue());
            IBridge result = bridge.SerializeArray(source);
            Assert.That(result.Value, Is.EqualTo(new BridgeValue(source)));
        }

        // =====================================================================
        // DeserializeGuid / SerializeGuid
        // =====================================================================

        [Test]
        public void DeserializeGuid_ValidGuidString_ReturnsMatchingGuid()
        {
            Guid guid = Guid.NewGuid();
            IBridge bridge = new Bridge(guid.ToString());
            Assert.That(bridge.DeserializeGuid(), Is.EqualTo(guid));
        }

        [Test]
        public void DeserializeGuid_InvalidGuidString_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge("not-a-guid");
            Assert.That(() => bridge.DeserializeGuid(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void DeserializeGuid_WrongType_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge(true);
            Assert.That(() => bridge.DeserializeGuid(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void SerializeGuid_Always_ReturnsBridgeWithMatchingValue()
        {
            Guid guid = Guid.NewGuid();
            IBridge bridge = new Bridge(new BridgeValue());
            IBridge result = bridge.SerializeGuid(guid);
            Assert.That(result.Value, Is.EqualTo((BridgeValue)guid.ToString()));
        }

        // =====================================================================
        // DeserializeDate
        // =====================================================================

        [Test]
        public void DeserializeDate_ValidDateString_ReturnsMatchingFamilyDate()
        {
            IBridge bridge = new Bridge("14 Aug 1985");
            Assert.That(bridge.DeserializeDate(), Is.EqualTo(new FamilyDate("1985", Month.Aug, 14)));
        }

        [Test]
        public void DeserializeDate_YearOnlyString_ReturnsMatchingFamilyDate()
        {
            IBridge bridge = new Bridge("1985");
            Assert.That(bridge.DeserializeDate(), Is.EqualTo(new FamilyDate("1985")));
        }

        [Test]
        public void DeserializeDate_InvalidDateString_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge("hello");
            Assert.That(() => bridge.DeserializeDate(), Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void DeserializeDate_WrongType_ThrowsInvalidCastException()
        {
            IBridge bridge = new Bridge(true);
            Assert.That(() => bridge.DeserializeDate(), Throws.TypeOf<InvalidCastException>());
        }
    }
}
