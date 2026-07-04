using System.Text.Json;
using VirtualFamilyMuseumLibrary.Serialization;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibraryTest.Serialization
{
    [TestFixture]
    public class BridgeSerializerTest
    {
        // =====================================================================
        // HandleNull
        // =====================================================================

        [Test]
        public void HandleNull_Always_ReturnsTrue()
        {
            BridgeSerializer serializer = new();
            Assert.That(serializer.HandleNull, Is.True);
        }

        // =====================================================================
        // Write — Null
        // =====================================================================

        [Test]
        public void Write_Null_WritesJsonNull()
        {
            IBridge bridge = new Bridge(new BridgeValue());
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("null"));
        }

        // =====================================================================
        // Write — String
        // =====================================================================

        [Test]
        public void Write_String_WritesQuotedJsonString()
        {
            IBridge bridge = new Bridge("hello");
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("\"hello\""));
        }

        // =====================================================================
        // Write — Number
        // =====================================================================

        [Test]
        public void Write_WholeNumberInIntRange_WritesWithoutDecimal()
        {
            IBridge bridge = new Bridge((Number)42.0);
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("42"));
        }

        [Test]
        public void Write_WholeNumberBeyondIntRange_WritesAsLong()
        {
            IBridge bridge = new Bridge((Number)3000000000.0);
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("3000000000"));
        }

        [Test]
        public void Write_DecimalNumber_WritesAsDouble()
        {
            IBridge bridge = new Bridge((Number)3.14);
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("3.14"));
        }

        [Test]
        public void Write_NegativeWholeNumber_WritesWithoutDecimal()
        {
            IBridge bridge = new Bridge((Number)(-7.0));
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("-7"));
        }

        // =====================================================================
        // Write — Bool
        // =====================================================================

        [Test]
        public void Write_True_WritesJsonTrue()
        {
            IBridge bridge = new Bridge(true);
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("true"));
        }

        [Test]
        public void Write_False_WritesJsonFalse()
        {
            IBridge bridge = new Bridge(false);
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("false"));
        }

        // =====================================================================
        // Write — Object
        // =====================================================================

        [Test]
        public void Write_ObjectWithSingleProperty_WritesJsonObject()
        {
            IBridge bridge = new Bridge(new BridgeValue(new Dictionary<string, BridgeValue> { ["a"] = "1" }));
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("{\"a\":\"1\"}"));
        }

        [Test]
        public void Write_EmptyObject_WritesEmptyJsonObject()
        {
            IBridge bridge = new Bridge(new BridgeValue(new Dictionary<string, BridgeValue>()));
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("{}"));
        }

        // =====================================================================
        // Write — Array
        // =====================================================================

        [Test]
        public void Write_ArrayWithElements_WritesJsonArray()
        {
            IBridge bridge = new Bridge(new BridgeValue(["1", "2"]));
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("[\"1\",\"2\"]"));
        }

        [Test]
        public void Write_EmptyArray_WritesEmptyJsonArray()
        {
            IBridge bridge = new Bridge(new BridgeValue([]));
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("[]"));
        }

        // =====================================================================
        // Write — Nested structures
        // =====================================================================

        [Test]
        public void Write_ObjectContainingArray_WritesNestedJson()
        {
            IBridge bridge = new Bridge(new BridgeValue(new Dictionary<string, BridgeValue>
            {
                ["items"] = new BridgeValue(["1", "2"])
            }));
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("{\"items\":[\"1\",\"2\"]}"));
        }

        [Test]
        public void Write_ArrayContainingObject_WritesNestedJson()
        {
            IBridge bridge = new Bridge(new BridgeValue(
            [
                new BridgeValue(new Dictionary<string, BridgeValue> { ["a"] = "1" })
            ]));
            string json = JsonSerializer.Serialize(bridge, SerializationExtensions.GetOptions(false));
            Assert.That(json, Is.EqualTo("[{\"a\":\"1\"}]"));
        }

        // =====================================================================
        // Read — Null
        // =====================================================================

        [Test]
        public void Read_JsonNull_ReturnsNullBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("null", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value.IsNull, Is.True);
        }

        // =====================================================================
        // Read — String
        // =====================================================================

        [Test]
        public void Read_JsonString_ReturnsStringBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("\"hello\"", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo((BridgeValue)"hello"));
        }

        // =====================================================================
        // Read — Number
        // =====================================================================

        [Test]
        public void Read_JsonWholeNumber_ReturnsNumberBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("42", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo((BridgeValue)(Number)42.0));
        }

        [Test]
        public void Read_JsonDecimalNumber_ReturnsNumberBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("3.14", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo((BridgeValue)(Number)3.14));
        }

        [Test]
        public void Read_JsonNegativeNumber_ReturnsNumberBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("-7", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo((BridgeValue)(Number)(-7.0)));
        }

        // =====================================================================
        // Read — Bool
        // =====================================================================

        [Test]
        public void Read_JsonTrue_ReturnsTrueBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("true", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo((BridgeValue)true));
        }

        [Test]
        public void Read_JsonFalse_ReturnsFalseBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("false", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo((BridgeValue)false));
        }

        // =====================================================================
        // Read — Object
        // =====================================================================

        [Test]
        public void Read_JsonObject_ReturnsObjectBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("{\"a\":\"1\"}", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value.AsObject["a"], Is.EqualTo((BridgeValue)"1"));
        }

        [Test]
        public void Read_EmptyJsonObject_ReturnsEmptyObjectBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("{}", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value.AsObject, Is.Empty);
        }

        [Test]
        public void Read_JsonObjectWithMultipleProperties_ReturnsAllEntries()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("{\"a\":\"1\",\"b\":2}", SerializationExtensions.GetOptions(false))!;
            IDictionary<string, BridgeValue> obj = result.Value.AsObject;
            Assert.That(obj["a"], Is.EqualTo((BridgeValue)"1"));
            Assert.That(obj["b"], Is.EqualTo((BridgeValue)(Number)2.0));
        }

        // =====================================================================
        // Read — Array
        // =====================================================================

        [Test]
        public void Read_JsonArray_ReturnsArrayBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("[\"1\",\"2\"]", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value.AsArray, Is.EqualTo(new List<BridgeValue> { "1", "2" }));
        }

        [Test]
        public void Read_EmptyJsonArray_ReturnsEmptyArrayBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("[]", SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value.AsArray, Is.Empty);
        }

        // =====================================================================
        // Read — Nested structures
        // =====================================================================

        [Test]
        public void Read_JsonObjectContainingArray_ReturnsNestedBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("{\"items\":[\"1\",\"2\"]}", SerializationExtensions.GetOptions(false))!;
            IDictionary<string, BridgeValue> obj = result.Value.AsObject;
            Assert.That(obj["items"].AsArray, Is.EqualTo(new List<BridgeValue> { "1", "2" }));
        }

        [Test]
        public void Read_JsonArrayContainingObject_ReturnsNestedBridge()
        {
            IBridge result = JsonSerializer.Deserialize<IBridge>("[{\"a\":\"1\"}]", SerializationExtensions.GetOptions(false))!;
            IList<BridgeValue> array = result.Value.AsArray;
            Assert.That(array[0].AsObject["a"], Is.EqualTo((BridgeValue)"1"));
        }

        // =====================================================================
        // Round-trip
        // =====================================================================

        [Test]
        public void RoundTrip_Null_PreservesValue()
        {
            IBridge original = new Bridge(new BridgeValue());
            string json = JsonSerializer.Serialize(original, SerializationExtensions.GetOptions(false));
            IBridge result = JsonSerializer.Deserialize<IBridge>(json, SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo(original.Value));
        }

        [Test]
        public void RoundTrip_String_PreservesValue()
        {
            IBridge original = new Bridge("hello");
            string json = JsonSerializer.Serialize(original, SerializationExtensions.GetOptions(false));
            IBridge result = JsonSerializer.Deserialize<IBridge>(json, SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo(original.Value));
        }

        [Test]
        public void RoundTrip_Number_PreservesValue()
        {
            IBridge original = new Bridge((Number)3.14);
            string json = JsonSerializer.Serialize(original, SerializationExtensions.GetOptions(false));
            IBridge result = JsonSerializer.Deserialize<IBridge>(json, SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo(original.Value));
        }

        [Test]
        public void RoundTrip_Bool_PreservesValue()
        {
            IBridge original = new Bridge(true);
            string json = JsonSerializer.Serialize(original, SerializationExtensions.GetOptions(false));
            IBridge result = JsonSerializer.Deserialize<IBridge>(json, SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo(original.Value));
        }

        [Test]
        public void RoundTrip_Object_PreservesValue()
        {
            IBridge original = new Bridge(new BridgeValue(new Dictionary<string, BridgeValue> { ["a"] = "1", ["b"] = (Number)2.0 }));
            string json = JsonSerializer.Serialize(original, SerializationExtensions.GetOptions(false));
            IBridge result = JsonSerializer.Deserialize<IBridge>(json, SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo(original.Value));
        }

        [Test]
        public void RoundTrip_Array_PreservesValue()
        {
            IBridge original = new Bridge(new BridgeValue(["1", (Number)2.0, true]));
            string json = JsonSerializer.Serialize(original, SerializationExtensions.GetOptions(false));
            IBridge result = JsonSerializer.Deserialize<IBridge>(json, SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo(original.Value));
        }

        [Test]
        public void RoundTrip_DeeplyNestedStructure_PreservesValue()
        {
            IBridge original = new Bridge(new BridgeValue(new Dictionary<string, BridgeValue>
            {
                ["name"] = "hello",
                ["count"] = (Number)3.0,
                ["active"] = true,
                ["tags"] = new BridgeValue(["x", "y"]),
                ["nested"] = new BridgeValue(new Dictionary<string, BridgeValue> { ["inner"] = new BridgeValue() })
            }));
            string json = JsonSerializer.Serialize(original, SerializationExtensions.GetOptions(false));
            IBridge result = JsonSerializer.Deserialize<IBridge>(json, SerializationExtensions.GetOptions(false))!;
            Assert.That(result.Value, Is.EqualTo(original.Value));
        }
    }
}
