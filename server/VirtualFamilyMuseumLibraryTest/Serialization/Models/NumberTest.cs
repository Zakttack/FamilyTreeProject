using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibraryTest.Serialization.Models
{
    [TestFixture]
    public class NumberTest
    {
                // =====================================================================
        // CompareTo
        // =====================================================================
        [Test]
        public void CompareTo_EqualValues_ReturnsZero()
        {
            Number a = 5.0;
            Number b = 5.0;
            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareTo_LessThan_ReturnsNegative()
        {
            Number a = 3.0;
            Number b = 7.0;
            Assert.That(a.CompareTo(b), Is.Negative);
        }

        [Test]
        public void CompareTo_GreaterThan_ReturnsPositive()
        {
            Number a = 7.0;
            Number b = 3.0;
            Assert.That(a.CompareTo(b), Is.Positive);
        }

        [Test]
        public void CompareTo_CloseDoubleValues_AreDistinct()
        {
            Number a = 1.5;
            Number b = 1.7;
            Assert.That(a.CompareTo(b), Is.Negative);
        }

        [Test]
        public void CompareTo_NegativeValues_ReturnsCorrectSign()
        {
            Number a = -10.0;
            Number b = -3.0;
            Assert.That(a.CompareTo(b), Is.Negative);
        }

        // =====================================================================
        // Equals
        // =====================================================================
        // 
        [Test]
        public void Equals_SameValue_ReturnsTrue()
        {
            Number a = 42.0;
            Number b = 42.0;
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            Number a = 42.0;
            Number b = 43.0;
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Equals_BoxedObject_SameValue_ReturnsTrue()
        {
            Number a = 10.0;
            object b = (Number)10.0;
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void Equals_BoxedObject_DifferentType_ReturnsFalse()
        {
            Number a = 10.0;
            Assert.That(a.Equals((object?)"10"), Is.False);
        }

        [Test]
        public void Equals_BoxedObject_Null_ReturnsFalse()
        {
            Number a = 10.0;
            Assert.That(a.Equals((object?)null), Is.False);
        }

        [Test]
        public void Equals_NegativeZeroAndZero_AreEqual()
        {
            Number a = 0.0;
            Number b = -0.0;
            Assert.That(a, Is.EqualTo(b));
        }

        // =====================================================================
        // GetHashCode
        // =====================================================================

        [Test]
        public void GetHashCode_EqualValues_ReturnSameHash()
        {
            Number a = 99.0;
            Number b = 99.0;
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_DifferentValues_ReturnDifferentHash()
        {
            Number a = 1.0;
            Number b = 2.0;
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        // =====================================================================
        // ToString
        // =====================================================================

        [Test]
        public void ToString_WholeDouble_ReturnsWithoutDecimal()
        {
            Number n = 5.0;
            Assert.That(n.ToString(), Is.EqualTo("5"));
        }

        [Test]
        public void ToString_DecimalValue_RetainsDecimal()
        {
            Number n = 3.14;
            Assert.That(n.ToString(), Is.EqualTo("3.14"));
        }

        [Test]
        public void ToString_NegativeWholeDouble_ReturnsWithoutDecimal()
        {
            Number n = -7.0;
            Assert.That(n.ToString(), Is.EqualTo("-7"));
        }

        [Test]
        public void ToString_Zero_ReturnsZero()
        {
            Number n = 0.0;
            Assert.That(n.ToString(), Is.EqualTo("0"));
        }

        [Test]
        public void ToString_LargeWholeDouble_ReturnsWithoutDecimal()
        {
            Number n = 1000000.0;
            Assert.That(n.ToString(), Is.EqualTo("1000000"));
        }

        // =====================================================================
        // TryGetDouble
        // =====================================================================

        [Test]
        public void TryGetDouble_Always_ReturnsTrue()
        {
            Number n = 3.14;
            Assert.That(n.TryGetDouble(out _), Is.True);
        }

        [Test]
        public void TryGetDouble_ReturnsCorrectValue()
        {
            Number n = 3.14;
            n.TryGetDouble(out double output);
            Assert.That(output, Is.EqualTo(3.14));
        }

        [Test]
        public void TryGetDouble_NegativeValue_ReturnsCorrectValue()
        {
            Number n = -2.5;
            n.TryGetDouble(out double output);
            Assert.That(output, Is.EqualTo(-2.5));
        }

        // =====================================================================
        // TryGetInt
        // =====================================================================

        [Test]
        public void TryGetInt_WholeValueInRange_ReturnsTrue()
        {
            Number n = 42.0;
            Assert.That(n.TryGetInt(out _), Is.True);
        }

        [Test]
        public void TryGetInt_WholeValueInRange_ReturnsCorrectValue()
        {
            Number n = 42.0;
            n.TryGetInt(out int output);
            Assert.That(output, Is.EqualTo(42));
        }

        [Test]
        public void TryGetInt_DecimalValue_ReturnsFalse()
        {
            Number n = 3.14;
            Assert.That(n.TryGetInt(out _), Is.False);
        }

        [Test]
        public void TryGetInt_DecimalValue_OutputIsZero()
        {
            Number n = 3.14;
            n.TryGetInt(out int output);
            Assert.That(output, Is.EqualTo(0));
        }

        [Test]
        public void TryGetInt_AboveMaxValue_ReturnsFalse()
        {
            Number n = (double)int.MaxValue + 1;
            Assert.That(n.TryGetInt(out _), Is.False);
        }

        [Test]
        public void TryGetInt_BelowMinValue_ReturnsFalse()
        {
            Number n = (double)int.MinValue - 1;
            Assert.That(n.TryGetInt(out _), Is.False);
        }

        [Test]
        public void TryGetInt_ExactMaxValue_ReturnsTrue()
        {
            Number n = (double)int.MaxValue;
            Assert.That(n.TryGetInt(out _), Is.True);
        }

        [Test]
        public void TryGetInt_ExactMinValue_ReturnsTrue()
        {
            Number n = (double)int.MinValue;
            Assert.That(n.TryGetInt(out _), Is.True);
        }

        [Test]
        public void TryGetInt_NegativeWholeValue_ReturnsCorrectValue()
        {
            Number n = -7.0;
            n.TryGetInt(out int output);
            Assert.That(output, Is.EqualTo(-7));
        }

        [Test]
        public void TryGetInt_Zero_ReturnsTrue()
        {
            Number n = 0.0;
            Assert.That(n.TryGetInt(out _), Is.True);
        }

        // =====================================================================
        // TryGetLong
        // =====================================================================

        [Test]
        public void TryGetLong_WholeValueInRange_ReturnsTrue()
        {
            // Avoid long.MaxValue directly — casting to double loses precision
            // and rounds beyond long.MaxValue, causing a false negative
            Number n = 9_000_000_000_000_000.0;
            Assert.That(n.TryGetLong(out _), Is.True);
        }

        [Test]
        public void TryGetLong_WholeValueInRange_ReturnsCorrectValue()
        {
            Number n = 100.0;
            n.TryGetLong(out long output);
            Assert.That(output, Is.EqualTo(100L));
        }

        [Test]
        public void TryGetLong_DecimalValue_ReturnsFalse()
        {
            Number n = 9.9;
            Assert.That(n.TryGetLong(out _), Is.False);
        }

        [Test]
        public void TryGetLong_DecimalValue_OutputIsZero()
        {
            Number n = 9.9;
            n.TryGetLong(out long output);
            Assert.That(output, Is.EqualTo(0L));
        }

        [Test]
        public void TryGetLong_NegativeWholeValue_ReturnsCorrectValue()
        {
            Number n = -500.0;
            n.TryGetLong(out long output);
            Assert.That(output, Is.EqualTo(-500L));
        }

        [Test]
        public void TryGetLong_Zero_ReturnsTrue()
        {
            Number n = 0.0;
            Assert.That(n.TryGetLong(out _), Is.True);
        }

        // =====================================================================
        // Implicit / Explicit Operators
        // =====================================================================

        [Test]
        public void ImplicitFromDouble_PreservesValue()
        {
            Number n = 3.14;
            n.TryGetDouble(out double output);
            Assert.That(output, Is.EqualTo(3.14));
        }

        [Test]
        public void ExplicitToDouble_PreservesValue()
        {
            Number n = 2.71;
            double output = (double)n;
            Assert.That(output, Is.EqualTo(2.71));
        }

        [Test]
        public void ImplicitFromLong_PreservesValue()
        {
            Number n = 123456789L;
            n.TryGetLong(out long output);
            Assert.That(output, Is.EqualTo(123456789L));
        }

        [Test]
        public void ExplicitToLong_WholeValue_Succeeds()
        {
            Number n = 99.0;
            long output = (long)n;
            Assert.That(output, Is.EqualTo(99L));
        }

        [Test]
        public void ExplicitToLong_DecimalValue_ThrowsInvalidCastException()
        {
            Number n = 1.5;
            Assert.That(() => (long)n, Throws.InvalidOperationException.Or.TypeOf<InvalidCastException>());
        }

        [Test]
        public void ExplicitToLong_AboveMaxValue_ThrowsInvalidCastException()
        {
            // double can represent values beyond long.MaxValue
            Number n = (double)long.MaxValue * 2;
            Assert.That(() => (long)n, Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void ExplicitToInt_WholeValue_Succeeds()
        {
            Number n = 7.0;
            int output = (int)n;
            Assert.That(output, Is.EqualTo(7));
        }

        [Test]
        public void ExplicitToInt_DecimalValue_ThrowsInvalidCastException()
        {
            Number n = 7.5;
            Assert.That(() => (int)n, Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void ExplicitToInt_AboveMaxValue_ThrowsInvalidCastException()
        {
            Number n = (double)int.MaxValue + 1;
            Assert.That(() => (int)n, Throws.TypeOf<InvalidCastException>());
        }

        [Test]
        public void ExplicitToInt_BelowMinValue_ThrowsInvalidCastException()
        {
            Number n = (double)int.MinValue - 1;
            Assert.That(() => (int)n, Throws.TypeOf<InvalidCastException>());
        }

        // =====================================================================
        // Comparison Operators
        // =====================================================================

        [Test]
        public void OperatorEqual_SameValues_ReturnsTrue()
        {
            Number a = 5.0;
            Number b = 5.0;
            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorNotEqual_DifferentValues_ReturnsTrue()
        {
            Number a = 5.0;
            Number b = 6.0;
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorLessThan_ReturnsTrue_WhenLess()
        {
            Number a = 1.0;
            Number b = 2.0;
            Assert.That(a < b, Is.True);
        }

        [Test]
        public void OperatorLessThan_ReturnsFalse_WhenGreater()
        {
            Number a = 2.0;
            Number b = 1.0;
            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorGreaterThan_ReturnsTrue_WhenGreater()
        {
            Number a = 3.0;
            Number b = 1.0;
            Assert.That(a > b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqual_EqualValues_ReturnsTrue()
        {
            Number a = 4.0;
            Number b = 4.0;
            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqual_EqualValues_ReturnsTrue()
        {
            Number a = 4.0;
            Number b = 4.0;
            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqual_GreaterValue_ReturnsTrue()
        {
            Number a = 5.0;
            Number b = 4.0;
            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqual_LessValue_ReturnsTrue()
        {
            Number a = 3.0;
            Number b = 4.0;
            Assert.That(a <= b, Is.True);
        }
    }
}