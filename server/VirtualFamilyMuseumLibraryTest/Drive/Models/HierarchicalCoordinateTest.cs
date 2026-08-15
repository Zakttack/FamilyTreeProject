using VirtualFamilyMuseumLibrary.Drive.Models;

namespace VirtualFamilyMuseumLibraryTest.Drive.Models
{
    public class HierarchicalCoordinateTest
    {
        // =====================================================================
        // Constructor
        // =====================================================================

        [Test]
        public void ConstructorShouldDefensivelyCopyInputArraySoExternalMutationsDoNotAffectCoordinate()
        {
            int[] source = [1, 2];
            HierarchicalCoordinate coordinate = new(source);

            source[1] = 99;

            Assert.That(coordinate.ToString(), Is.EqualTo("1.2)"));
        }

        // =====================================================================
        // Parent
        // =====================================================================

        [Test]
        public void ParentShouldReturnNullForEmptyCoordinate()
        {
            HierarchicalCoordinate root = new([]);
            Assert.That(root.Parent, Is.Null);
        }

        [Test]
        public void ParentShouldReturnEmptyCoordinateForTopLevelCoordinate()
        {
            HierarchicalCoordinate coordinate = new([1]);
            Assert.That(coordinate.Parent, Is.Not.Null);
            Assert.That(coordinate.Parent!.Value.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void ParentShouldRemoveLastSegmentForNestedCoordinate()
        {
            HierarchicalCoordinate coordinate = new([1, 2, 3]);
            Assert.That(coordinate.Parent!.Value.ToString(), Is.EqualTo("1.2)"));
        }

        // =====================================================================
        // Child
        // =====================================================================

        [Test]
        public void ChildOfEmptyCoordinateShouldReturnFirstTopLevelCoordinate()
        {
            HierarchicalCoordinate root = new([]);
            Assert.That(root.Child.ToString(), Is.EqualTo("1)"));
        }

        [Test]
        public void ChildShouldAppendFirstSegmentToExistingCoordinate()
        {
            HierarchicalCoordinate coordinate = new([1, 2]);
            Assert.That(coordinate.Child.ToString(), Is.EqualTo("1.2.1)"));
        }

        // =====================================================================
        // NextSibling
        // =====================================================================

        [Test]
        public void NextSiblingShouldReturnNullForEmptyCoordinate()
        {
            HierarchicalCoordinate root = new([]);
            Assert.That(root.NextSibling, Is.Null);
        }

        [Test]
        public void NextSiblingShouldIncrementLastSegmentOnly()
        {
            HierarchicalCoordinate coordinate = new([2, 5]);
            Assert.That(coordinate.NextSibling!.Value.ToString(), Is.EqualTo("2.6)"));
        }

        [Test]
        public void NextSiblingShouldNotMutateOriginalCoordinate()
        {
            HierarchicalCoordinate coordinate = new([1, 2, 3]);
            HierarchicalCoordinate sibling = coordinate.NextSibling!.Value;

            Assert.That(coordinate.ToString(), Is.EqualTo("1.2.3)"));
            Assert.That(sibling.ToString(), Is.EqualTo("1.2.4)"));
        }

        // =====================================================================
        // CompareTo
        // =====================================================================

        [Test]
        public void CompareToShouldReturnZeroForBothEmptyCoordinates()
        {
            HierarchicalCoordinate a = new([]);
            HierarchicalCoordinate b = new([]);

            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareToShouldReturnNegativeWhenEmptyComparedToNonEmpty()
        {
            HierarchicalCoordinate a = new([]);
            HierarchicalCoordinate b = new([1]);

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldReturnPositiveWhenNonEmptyComparedToEmpty()
        {
            HierarchicalCoordinate a = new([1]);
            HierarchicalCoordinate b = new([]);

            Assert.That(a.CompareTo(b), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldReturnZeroForIdenticalCoordinates()
        {
            HierarchicalCoordinate a = new([1, 1, 2]);
            HierarchicalCoordinate b = new([1, 1, 2]);

            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareToShouldOrderParentBeforeChild()
        {
            HierarchicalCoordinate parent = new([1]);
            HierarchicalCoordinate child = new([1, 1]);

            Assert.That(parent.CompareTo(child), Is.LessThan(0));
            Assert.That(child.CompareTo(parent), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldOrderDeeplyNestedDescendantBeforeNextSibling()
        {
            HierarchicalCoordinate deeplyNested = new([1, 1, 1]);
            HierarchicalCoordinate nextSibling = new([1, 2]);

            Assert.That(deeplyNested.CompareTo(nextSibling), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldOrderSiblingsBySegmentValueNumericallyNotLexicographically()
        {
            HierarchicalCoordinate nine = new([1, 9]);
            HierarchicalCoordinate ten = new([1, 10]);

            Assert.That(nine.CompareTo(ten), Is.LessThan(0));
            Assert.That(ten.CompareTo(nine), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldReturnNegativeForEarlierSegmentAtSameDepth()
        {
            HierarchicalCoordinate a = new([1, 1]);
            HierarchicalCoordinate b = new([1, 2]);

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldReturnPositiveForLaterSegmentAtSameDepth()
        {
            HierarchicalCoordinate a = new([2]);
            HierarchicalCoordinate b = new([1]);

            Assert.That(a.CompareTo(b), Is.GreaterThan(0));
        }

        // =====================================================================
        // Equals (typed and object overload) + GetHashCode
        // =====================================================================

        [Test]
        public void EqualsShouldReturnTrueForCoordinatesWithSameSegments()
        {
            HierarchicalCoordinate a = new([1, 2, 3]);
            HierarchicalCoordinate b = new([1, 2, 3]);

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsShouldReturnFalseForCoordinatesWithDifferentSegments()
        {
            HierarchicalCoordinate a = new([1, 2, 3]);
            HierarchicalCoordinate b = new([1, 2, 4]);

            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void EqualsObjectOverloadShouldReturnTrueForBoxedEqualCoordinate()
        {
            HierarchicalCoordinate a = new([1, 2]);
            object b = new HierarchicalCoordinate([1, 2]);

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsObjectOverloadShouldReturnFalseForNonHierarchicalCoordinateOrNull()
        {
            HierarchicalCoordinate a = new([1]);

            Assert.That(a.Equals("1)"), Is.False);
            Assert.That(a.Equals(null), Is.False);
            Assert.That(a.Equals(1), Is.False);
        }

        [Test]
        public void GetHashCodeShouldBeConsistentForEqualCoordinatesBackedByDistinctArrayInstances()
        {
            HierarchicalCoordinate a = new([1, 2, 3]);
            HierarchicalCoordinate b = new([1, 2, 3]);

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCodeShouldDifferForDifferentCoordinates()
        {
            HierarchicalCoordinate a = new([1, 2, 3]);
            HierarchicalCoordinate b = new([4, 5, 6]);

            // Hash collisions are possible in theory but extremely unlikely for these values
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        // =====================================================================
        // ToString
        // =====================================================================

        [Test]
        public void ToStringShouldReturnEmptyStringForEmptyCoordinate()
        {
            HierarchicalCoordinate root = new([]);
            Assert.That(root.ToString(), Is.EqualTo(""));
        }

        [Test]
        public void ToStringShouldReturnSingleSegmentWithClosingParenthesis()
        {
            HierarchicalCoordinate coordinate = new([1]);
            Assert.That(coordinate.ToString(), Is.EqualTo("1)"));
        }

        [Test]
        public void ToStringShouldReturnDotSeparatedSegmentsWithClosingParenthesis()
        {
            HierarchicalCoordinate coordinate = new([1, 1, 2, 1, 1]);
            Assert.That(coordinate.ToString(), Is.EqualTo("1.1.2.1.1)"));
        }

        // =====================================================================
        // Comparison operators — both non-null operands
        // =====================================================================

        [Test]
        public void OperatorEqualsShouldReturnTrueForIdenticalCoordinates()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1, 1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1, 1]);

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseForDifferentCoordinates()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([2]);

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnFalseForIdenticalCoordinates()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1, 1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1, 1]);

            Assert.That(a != b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueForDifferentCoordinates()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([2]);

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnTrueWhenFirstIsEarlier()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([2]);

            Assert.That(a < b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenFirstIsLater()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([2]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenCoordinatesAreEqual()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenFirstIsEarlier()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([2]);

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenCoordinatesAreEqual()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnFalseWhenFirstIsLater()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([2]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a <= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnTrueWhenFirstIsLater()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([2]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a > b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenFirstIsEarlier()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([2]);

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenCoordinatesAreEqual()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenFirstIsLater()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([2]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenCoordinatesAreEqual()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnFalseWhenFirstIsEarlier()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = new HierarchicalCoordinate([2]);

            Assert.That(a >= b, Is.False);
        }

        // =====================================================================
        // Comparison operators — null operands
        // =====================================================================

        [Test]
        public void OperatorEqualsShouldReturnTrueForBothNull()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = null;

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseWhenOnlyLeftIsNull()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseWhenOnlyRightIsNull()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = null;

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnFalseForBothNull()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = null;

            Assert.That(a != b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueWhenOnlyLeftIsNull()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueWhenOnlyRightIsNull()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = null;

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseForBothNull()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = null;

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanShouldReturnTrueWhenLeftIsNullAndRightIsNot()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a < b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenRightIsNullAndLeftIsNot()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = null;

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueForBothNull()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = null;

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenLeftIsNullAndRightIsNot()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnFalseWhenRightIsNullAndLeftIsNot()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = null;

            Assert.That(a <= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseForBothNull()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = null;

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenLeftIsNullAndRightIsNot()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnTrueWhenRightIsNullAndLeftIsNot()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = null;

            Assert.That(a > b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueForBothNull()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = null;

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnFalseWhenLeftIsNullAndRightIsNot()
        {
            HierarchicalCoordinate? a = null;
            HierarchicalCoordinate? b = new HierarchicalCoordinate([1]);

            Assert.That(a >= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenRightIsNullAndLeftIsNot()
        {
            HierarchicalCoordinate? a = new HierarchicalCoordinate([1]);
            HierarchicalCoordinate? b = null;

            Assert.That(a >= b, Is.True);
        }
    }
}
