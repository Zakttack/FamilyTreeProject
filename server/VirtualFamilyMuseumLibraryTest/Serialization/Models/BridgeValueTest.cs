using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibraryTest.Serialization.Models
{
    [TestFixture]
    public class BridgeValueTest
    {
        // =====================================================================
        // IsNull
        // =====================================================================

        [Test]
        public void IsNull_DefaultConstructor_ReturnsTrue()
        {
            BridgeValue value = new();
            Assert.That(value.IsNull, Is.True);
        }

        [Test]
        public void IsNull_ParameterlessConstructor_ReturnsTrue()
        {
            BridgeValue value = new();
            Assert.That(value.IsNull, Is.True);
        }

        [Test]
        public void IsNull_String_ReturnsFalse()
        {
            BridgeValue value = "hello";
            Assert.That(value.IsNull, Is.False);
        }

        [Test]
        public void IsNull_Number_ReturnsFalse()
        {
            BridgeValue value = (Number)5.0;
            Assert.That(value.IsNull, Is.False);
        }

        [Test]
        public void IsNull_Bool_ReturnsFalse()
        {
            BridgeValue value = true;
            Assert.That(value.IsNull, Is.False);
        }

        [Test]
        public void IsNull_Object_ReturnsFalse()
        {
            BridgeValue value = new(new Dictionary<string, BridgeValue>());
            Assert.That(value.IsNull, Is.False);
        }

        [Test]
        public void IsNull_Array_ReturnsFalse()
        {
            BridgeValue value = new(new List<BridgeValue>());
            Assert.That(value.IsNull, Is.False);
        }

        // =====================================================================
        // AsObject
        // =====================================================================

        [Test]
        public void AsObject_Null_ThrowsInvalidCastException()
        {
            BridgeValue value = new();
            Assert.That(() => value.AsObject, Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void AsObject_WrongType_ThrowsInvalidCastException()
        {
            BridgeValue value = "hello";
            Assert.That(() => value.AsObject, Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void AsObject_Object_ReturnsMatchingEntries()
        {
            IDictionary<string, BridgeValue> source = new Dictionary<string, BridgeValue> { ["a"] = "1" };
            BridgeValue value = new(source);
            Assert.That(value.AsObject["a"], Is.EqualTo((BridgeValue)"1"));
        }

        [Test]
        public void AsObject_MutatingReturnedCopy_DoesNotAffectOriginal()
        {
            IDictionary<string, BridgeValue> source = new Dictionary<string, BridgeValue> { ["a"] = "1" };
            BridgeValue value = new(source);
            value.AsObject["a"] = "2";
            Assert.That(value.AsObject["a"], Is.EqualTo((BridgeValue)"1"));
        }

        // =====================================================================
        // AsArray
        // =====================================================================

        [Test]
        public void AsArray_Null_ThrowsInvalidCastException()
        {
            BridgeValue value = new();
            Assert.That(() => value.AsArray, Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void AsArray_WrongType_ThrowsInvalidCastException()
        {
            BridgeValue value = "hello";
            Assert.That(() => value.AsArray, Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void AsArray_Array_ReturnsMatchingElements()
        {
            IList<BridgeValue> source = ["1", "2"];
            BridgeValue value = new(source);
            Assert.That(value.AsArray, Is.EqualTo(source));
        }

        [Test]
        public void AsArray_MutatingReturnedCopy_DoesNotAffectOriginal()
        {
            IList<BridgeValue> source = ["1"];
            BridgeValue value = new(source);
            value.AsArray.Add("2");
            Assert.That(value.AsArray, Has.Count.EqualTo(1));
        }

        // =====================================================================
        // Equals
        // =====================================================================

        [Test]
        public void Equals_BothNull_ReturnsTrue()
        {
            BridgeValue a = new();
            BridgeValue b = new();
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_NullAndNonNull_ReturnsFalse()
        {
            BridgeValue a = new();
            BridgeValue b = "hello";
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_NonNullAndNull_ReturnsFalse()
        {
            BridgeValue a = "hello";
            BridgeValue b = new();
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_SameStrings_ReturnsTrue()
        {
            BridgeValue a = "hello";
            BridgeValue b = "hello";
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_DifferentStrings_ReturnsFalse()
        {
            BridgeValue a = "hello";
            BridgeValue b = "world";
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_SameNumbers_ReturnsTrue()
        {
            BridgeValue a = (Number)5.0;
            BridgeValue b = (Number)5.0;
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_DifferentNumbers_ReturnsFalse()
        {
            BridgeValue a = (Number)5.0;
            BridgeValue b = (Number)6.0;
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_SameBools_ReturnsTrue()
        {
            BridgeValue a = true;
            BridgeValue b = true;
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_DifferentBools_ReturnsFalse()
        {
            BridgeValue a = true;
            BridgeValue b = false;
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_DifferentTypes_ReturnsFalse()
        {
            BridgeValue a = "5";
            BridgeValue b = (Number)5.0;
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_SameObjectContents_ReturnsTrue()
        {
            BridgeValue a = new(new Dictionary<string, BridgeValue> { ["a"] = "1", ["b"] = "2" });
            BridgeValue b = new(new Dictionary<string, BridgeValue> { ["a"] = "1", ["b"] = "2" });
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_ObjectContentsInDifferentOrder_ReturnsTrue()
        {
            BridgeValue a = new(new Dictionary<string, BridgeValue> { ["a"] = "1", ["b"] = "2" });
            BridgeValue b = new(new Dictionary<string, BridgeValue> { ["b"] = "2", ["a"] = "1" });
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_ObjectDifferentCount_ReturnsFalse()
        {
            BridgeValue a = new(new Dictionary<string, BridgeValue> { ["a"] = "1", ["b"] = "2" });
            BridgeValue b = new(new Dictionary<string, BridgeValue> { ["a"] = "1" });
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_ObjectMissingKey_ReturnsFalse()
        {
            BridgeValue a = new(new Dictionary<string, BridgeValue> { ["a"] = "1" });
            BridgeValue b = new(new Dictionary<string, BridgeValue> { ["b"] = "1" });
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_ObjectDifferentValueForSameKey_ReturnsFalse()
        {
            BridgeValue a = new(new Dictionary<string, BridgeValue> { ["a"] = "1" });
            BridgeValue b = new(new Dictionary<string, BridgeValue> { ["a"] = "2" });
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_SameArrayContentsSameOrder_ReturnsTrue()
        {
            BridgeValue a = new(["1", "2"]);
            BridgeValue b = new(["1", "2"]);
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_SameArrayContentsDifferentOrder_ReturnsFalse()
        {
            BridgeValue a = new(["1", "2"]);
            BridgeValue b = new(["2", "1"]);
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_ArrayDifferentLength_ReturnsFalse()
        {
            BridgeValue a = new(["1", "2"]);
            BridgeValue b = new(["1"]);
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_ObjectOverload_MatchingBridgeValue_ReturnsTrue()
        {
            BridgeValue a = "hello";
            object b = (BridgeValue)"hello";
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_ObjectOverload_NonBridgeValue_ReturnsFalse()
        {
            BridgeValue a = "hello";
            Assert.That(a.Equals((object?)"hello"), Is.False);
        }

        [Test]
        public void Equals_ObjectOverload_Null_ReturnsFalse()
        {
            BridgeValue a = "hello";
            Assert.That(a, Is.Not.EqualTo((object?)null));
        }

        // =====================================================================
        // GetHashCode
        // =====================================================================

        [Test]
        public void GetHashCode_Null_ReturnsZero()
        {
            BridgeValue value = new();
            Assert.That(value.GetHashCode(), Is.EqualTo(0));
        }

        [Test]
        public void GetHashCode_EqualStrings_ReturnSameHash()
        {
            BridgeValue a = "hello";
            BridgeValue b = "hello";
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_EqualNumbers_ReturnSameHash()
        {
            BridgeValue a = (Number)5.0;
            BridgeValue b = (Number)5.0;
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_EqualBools_ReturnSameHash()
        {
            BridgeValue a = true;
            BridgeValue b = true;
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_ObjectContentsInDifferentOrder_ReturnSameHash()
        {
            BridgeValue a = new(new Dictionary<string, BridgeValue> { ["a"] = "1", ["b"] = "2" });
            BridgeValue b = new(new Dictionary<string, BridgeValue> { ["b"] = "2", ["a"] = "1" });
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_ArrayContentsInDifferentOrder_ReturnDifferentHash()
        {
            BridgeValue a = new(["1", "2"]);
            BridgeValue b = new(["2", "1"]);
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_EqualArrays_ReturnSameHash()
        {
            BridgeValue a = new(["1", "2"]);
            BridgeValue b = new(["1", "2"]);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        // =====================================================================
        // ToString
        // =====================================================================

        [Test]
        public void ToString_Null_ReturnsJsonNull()
        {
            BridgeValue value = new();
            Assert.That(value.ToString(), Is.EqualTo("null"));
        }

        [Test]
        public void ToString_String_ReturnsQuotedJsonString()
        {
            BridgeValue value = "hello";
            Assert.That(value.ToString(), Is.EqualTo("\"hello\""));
        }

        [Test]
        public void ToString_Bool_ReturnsJsonBool()
        {
            BridgeValue value = true;
            Assert.That(value.ToString(), Is.EqualTo("true"));
        }

        // =====================================================================
        // TryAsString
        // =====================================================================

        [Test]
        public void TryAsString_String_ReturnsTrue()
        {
            BridgeValue value = "hello";
            Assert.That(value.TryAsString(out _), Is.True);
        }

        [Test]
        public void TryAsString_String_OutputsValue()
        {
            BridgeValue value = "hello";
            value.TryAsString(out string? text);
            Assert.That(text, Is.EqualTo("hello"));
        }

        [Test]
        public void TryAsString_WrongType_ReturnsFalse()
        {
            BridgeValue value = true;
            Assert.That(value.TryAsString(out _), Is.False);
        }

        [Test]
        public void TryAsString_Null_ReturnsFalse()
        {
            BridgeValue value = new();
            Assert.That(value.TryAsString(out _), Is.False);
        }

        // =====================================================================
        // TryAsNumber
        // =====================================================================

        [Test]
        public void TryAsNumber_Number_ReturnsTrue()
        {
            BridgeValue value = (Number)5.0;
            Assert.That(value.TryAsNumber(out _), Is.True);
        }

        [Test]
        public void TryAsNumber_Number_OutputsValue()
        {
            BridgeValue value = (Number)5.0;
            value.TryAsNumber(out Number? number);
            Assert.That(number, Is.EqualTo((Number)5.0));
        }

        [Test]
        public void TryAsNumber_WrongType_ReturnsFalse()
        {
            BridgeValue value = "5";
            Assert.That(value.TryAsNumber(out _), Is.False);
        }

        // =====================================================================
        // TryAsBool
        // =====================================================================

        [Test]
        public void TryAsBool_Bool_ReturnsTrue()
        {
            BridgeValue value = true;
            Assert.That(value.TryAsBool(out _), Is.True);
        }

        [Test]
        public void TryAsBool_Bool_OutputsValue()
        {
            BridgeValue value = true;
            value.TryAsBool(out bool? result);
            Assert.That(result, Is.True);
        }

        [Test]
        public void TryAsBool_WrongType_ReturnsFalse()
        {
            BridgeValue value = "true";
            Assert.That(value.TryAsBool(out _), Is.False);
        }

        // =====================================================================
        // TryAsObject
        // =====================================================================

        [Test]
        public void TryAsObject_Object_ReturnsTrue()
        {
            BridgeValue value = new(new Dictionary<string, BridgeValue> { ["a"] = "1" });
            Assert.That(value.TryAsObject(out _), Is.True);
        }

        [Test]
        public void TryAsObject_Object_OutputsMatchingEntries()
        {
            BridgeValue value = new(new Dictionary<string, BridgeValue> { ["a"] = "1" });
            value.TryAsObject(out IDictionary<string, BridgeValue>? obj);
            Assert.That(obj!["a"], Is.EqualTo((BridgeValue)"1"));
        }

        [Test]
        public void TryAsObject_WrongType_ReturnsFalse()
        {
            BridgeValue value = "hello";
            Assert.That(value.TryAsObject(out _), Is.False);
        }

        // =====================================================================
        // TryAsArray
        // =====================================================================

        [Test]
        public void TryAsArray_Array_ReturnsTrue()
        {
            BridgeValue value = new(new List<BridgeValue> { "1" });
            Assert.That(value.TryAsArray(out _), Is.True);
        }

        [Test]
        public void TryAsArray_Array_OutputsMatchingElements()
        {
            IList<BridgeValue> source = new List<BridgeValue> { "1", "2" };
            BridgeValue value = new(source);
            value.TryAsArray(out IList<BridgeValue>? array);
            Assert.That(array, Is.EqualTo(source));
        }

        [Test]
        public void TryAsArray_WrongType_ReturnsFalse()
        {
            BridgeValue value = "hello";
            Assert.That(value.TryAsArray(out _), Is.False);
        }

        // =====================================================================
        // Implicit / Explicit Operators
        // =====================================================================

        [Test]
        public void ImplicitFromString_PreservesValue()
        {
            BridgeValue value = "hello";
            Assert.Multiple(() =>
            {
                Assert.That(value.TryAsString(out string? text), Is.True);
                Assert.That(text, Is.EqualTo("hello"));
            });
        }

        [Test]
        public void ExplicitToString_PreservesValue()
        {
            BridgeValue value = "hello";
            string output = (string)value;
            Assert.That(output, Is.EqualTo("hello"));
        }

        [Test]
        public void ExplicitToString_WrongType_ThrowsInvalidCastException()
        {
            BridgeValue value = true;
            Assert.That(() => (string)value, Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void ImplicitFromNumber_PreservesValue()
        {
            BridgeValue value = (Number)5.0;
            Assert.That(value.TryAsNumber(out Number? number), Is.True);
            Assert.That(number, Is.EqualTo((Number)5.0));
        }

        [Test]
        public void ExplicitToNumber_PreservesValue()
        {
            BridgeValue value = (Number)5.0;
            Number output = (Number)value;
            Assert.That(output, Is.EqualTo((Number)5.0));
        }

        [Test]
        public void ExplicitToNumber_WrongType_ThrowsInvalidCastException()
        {
            BridgeValue value = "5";
            Assert.That(() => (Number)value, Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void ImplicitFromBool_PreservesValue()
        {
            BridgeValue value = true;
            Assert.Multiple(() =>
            {
                Assert.That(value.TryAsBool(out bool? result), Is.True);
                Assert.That(result, Is.True);
            });
        }

        [Test]
        public void ExplicitToBool_PreservesValue()
        {
            BridgeValue value = false;
            bool output = (bool)value;
            Assert.That(output, Is.False);
        }

        [Test]
        public void ExplicitToBool_WrongType_ThrowsInvalidCastException()
        {
            BridgeValue value = "true";
            Assert.That(() => (bool)value, Throws.TypeOf<InvalidCastException>());
        }

        // =====================================================================
        // Comparison Operators
        // =====================================================================

        [Test]
        public void OperatorEqual_SameValues_ReturnsTrue()
        {
            BridgeValue a = "hello";
            BridgeValue b = "hello";
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void OperatorEqual_DifferentValues_ReturnsFalse()
        {
            BridgeValue a = "hello";
            BridgeValue b = "world";
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void OperatorNotEqual_DifferentValues_ReturnsTrue()
        {
            BridgeValue a = "hello";
            BridgeValue b = "world";
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void OperatorNotEqual_SameValues_ReturnsFalse()
        {
            BridgeValue a = "hello";
            BridgeValue b = "hello";
            Assert.That(a, Is.EqualTo(b));
        }
    }
}
