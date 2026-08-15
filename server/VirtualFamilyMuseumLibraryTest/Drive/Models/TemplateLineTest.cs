using VirtualFamilyMuseumLibrary.Drive.Models;
using VirtualFamilyMuseumLibrary.Models;

namespace VirtualFamilyMuseumLibraryTest.Drive.Models
{
    public class TemplateLineTest
    {
        // =====================================================================
        // ToString
        // =====================================================================

        [Test]
        public void ToStringShouldReturnDashForMemberWithNoDatesAndNoInLaw()
        {
            TemplateLine line = new()
            {
                Coordinate = new([1, 1]),
                MemberBirthName = "Kit Dale Kessler",
            };

            Assert.That(line.ToString(), Is.EqualTo("1.1) Kit Dale Kessler (–)"));
        }

        [Test]
        public void ToStringShouldReturnPresentForMemberWithBirthDateButNoDeceasedDateAndNoInLaw()
        {
            TemplateLine line = new()
            {
                Coordinate = new([1]),
                MemberBirthName = "Todd Solo",
                MemberBirthDate = new FamilyDate("1975"),
            };

            Assert.That(line.ToString(), Is.EqualTo("1) Todd Solo (1975 – Present)"));
        }

        [Test]
        public void ToStringShouldReturnBirthAndDeceasedDateForMemberWithBothDatesAndNoInLaw()
        {
            TemplateLine line = new()
            {
                Coordinate = new([2]),
                MemberBirthName = "Colby Bryan Kessler",
                MemberBirthDate = new FamilyDate("1888", Month.Feb, 20),
                MemberDeceasedDate = new FamilyDate("1942", Month.Apr),
            };

            Assert.That(line.ToString(), Is.EqualTo("2) Colby Bryan Kessler (20 Feb 1888 – Apr 1942)"));
        }

        [Test]
        public void ToStringShouldReturnFullLineMatchingSampleTemplateFormat()
        {
            TemplateLine line = new()
            {
                Coordinate = new([1]),
                MemberBirthName = "Brian Bryan Kessler",
                MemberBirthDate = new FamilyDate("1886", Month.Sep),
                MemberDeceasedDate = new FamilyDate("1915", Month.Dec),
                InLawBirthName = "Todd Zachary Vasterling",
                InLawBirthDate = new FamilyDate("1911"),
                FamilyDynamicStartDate = new FamilyDate("1948"),
            };

            Assert.That(line.ToString(), Is.EqualTo("1) Brian Bryan Kessler (Sep 1886 – Dec 1915) & Todd Zachary Vasterling (1911 – Present): 1948"));
        }

        [Test]
        public void ToStringShouldReturnDashForMemberAndPresentForInLawWithFamilyDynamicStartDate()
        {
            TemplateLine line = new()
            {
                Coordinate = new([1, 2]),
                MemberBirthName = "Lisa Kaylynn Kessler",
                InLawBirthName = "Hallie Jordon Delacroix",
                InLawBirthDate = new FamilyDate("1927", Month.Jan, 21),
                FamilyDynamicStartDate = new FamilyDate("1967", Month.May, 11),
            };

            Assert.That(line.ToString(), Is.EqualTo("1.2) Lisa Kaylynn Kessler (–) & Hallie Jordon Delacroix (21 Jan 1927 – Present): 11 May 1967"));
        }

        [Test]
        public void ToStringShouldOmitFamilyDynamicStartDateWhenNotSet()
        {
            TemplateLine line = new()
            {
                Coordinate = new([2, 1]),
                MemberBirthName = "Dorothy Ryleigh Kessler",
                MemberBirthDate = new FamilyDate("1936"),
                MemberDeceasedDate = new FamilyDate("1991", Month.Dec, 13),
                InLawBirthName = "Ella Nicole Kessler",
                InLawBirthDate = new FamilyDate("1929"),
                InLawDeceasedDate = new FamilyDate("1961", Month.Apr, 17),
            };

            Assert.That(line.ToString(), Is.EqualTo("2.1) Dorothy Ryleigh Kessler (1936 – 13 Dec 1991) & Ella Nicole Kessler (1929 – 17 Apr 1961)"));
        }

        [Test]
        public void ToStringShouldReturnDashForInLawWithNoDates()
        {
            TemplateLine line = new()
            {
                Coordinate = new([3]),
                MemberBirthName = "Steven Chris Kessler",
                InLawBirthName = "Chris Colby Renquist",
            };

            Assert.That(line.ToString(), Is.EqualTo("3) Steven Chris Kessler (–) & Chris Colby Renquist (–)"));
        }

        // =====================================================================
        // CompareTo
        // =====================================================================

        [Test]
        public void CompareToShouldReturnPositiveWhenOtherIsNull()
        {
            TemplateLine line = new() { Coordinate = new([1]), MemberBirthName = "Solo" };
            Assert.That(line.CompareTo(null), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldReturnZeroForFullyIdenticalLines()
        {
            TemplateLine a = new()
            {
                Coordinate = new([1, 1]),
                MemberBirthName = "Same Name",
                MemberBirthDate = new FamilyDate("1980"),
                MemberDeceasedDate = new FamilyDate("2000"),
                InLawBirthName = "Same Spouse",
                InLawBirthDate = new FamilyDate("1981"),
                InLawDeceasedDate = new FamilyDate("2001"),
                FamilyDynamicStartDate = new FamilyDate("2000"),
            };
            TemplateLine b = new()
            {
                Coordinate = new([1, 1]),
                MemberBirthName = "Same Name",
                MemberBirthDate = new FamilyDate("1980"),
                MemberDeceasedDate = new FamilyDate("2000"),
                InLawBirthName = "Same Spouse",
                InLawBirthDate = new FamilyDate("1981"),
                InLawDeceasedDate = new FamilyDate("2001"),
                FamilyDynamicStartDate = new FamilyDate("2000"),
            };

            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareToShouldOrderByCoordinateFirst()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Zed Name" };
            TemplateLine b = new() { Coordinate = new([2]), MemberBirthName = "Abe Name" };

            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(b.CompareTo(a), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldOrderByMemberBirthNameWhenCoordinatesMatch()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Adams" };
            TemplateLine b = new() { Coordinate = new([1]), MemberBirthName = "Baker" };

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldOrderByMemberBirthDateWhenNamesMatch()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same", MemberBirthDate = new FamilyDate("1980") };
            TemplateLine b = new() { Coordinate = new([1]), MemberBirthName = "Same", MemberBirthDate = new FamilyDate("1990") };

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldOrderByMemberDeceasedDateWhenBirthDatesMatch()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same", MemberBirthDate = new FamilyDate("1980"), MemberDeceasedDate = new FamilyDate("2000") };
            TemplateLine b = new() { Coordinate = new([1]), MemberBirthName = "Same", MemberBirthDate = new FamilyDate("1980"), MemberDeceasedDate = new FamilyDate("2010") };

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldOrderByFamilyDynamicStartDateWhenMemberFieldsMatch()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Spouse", FamilyDynamicStartDate = new FamilyDate("1990") };
            TemplateLine b = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Spouse", FamilyDynamicStartDate = new FamilyDate("2000") };

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldOrderLineWithoutInLawBeforeLineWithInLaw()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same" };
            TemplateLine b = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Someone" };

            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(b.CompareTo(a), Is.GreaterThan(0));
        }

        [Test]
        public void CompareToShouldOrderByInLawBirthNameWhenBothPresentAndDiffer()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Adams" };
            TemplateLine b = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Baker" };

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        [Test]
        public void CompareToShouldOrderByInLawBirthDateWhenInLawNamesMatch()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Same Spouse", InLawBirthDate = new FamilyDate("1980") };
            TemplateLine b = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Same Spouse", InLawBirthDate = new FamilyDate("1990") };

            // Regression: InLawBirthDate must still be compared when InLawBirthName matches.
            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void CompareToShouldOrderByInLawDeceasedDateWhenInLawNamesAndBirthDatesMatch()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Same Spouse", InLawBirthDate = new FamilyDate("1980"), InLawDeceasedDate = new FamilyDate("2000") };
            TemplateLine b = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Same Spouse", InLawBirthDate = new FamilyDate("1980"), InLawDeceasedDate = new FamilyDate("2010") };

            Assert.That(a.CompareTo(b), Is.LessThan(0));
        }

        // =====================================================================
        // Equals (typed and object overload) + GetHashCode
        // =====================================================================

        [Test]
        public void EqualsShouldReturnTrueForLinesWithIdenticalFields()
        {
            TemplateLine a = new() { Coordinate = new([1, 1]), MemberBirthName = "Same Name", MemberBirthDate = new FamilyDate("1980") };
            TemplateLine b = new() { Coordinate = new([1, 1]), MemberBirthName = "Same Name", MemberBirthDate = new FamilyDate("1980") };

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsShouldReturnFalseWhenInLawBirthDateDiffersEvenIfInLawNameMatches()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Same Spouse", InLawBirthDate = new FamilyDate("1980") };
            TemplateLine b = new() { Coordinate = new([1]), MemberBirthName = "Same", InLawBirthName = "Same Spouse", InLawBirthDate = new FamilyDate("1990") };

            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void EqualsObjectOverloadShouldReturnTrueForEqualTemplateLine()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            object b = new TemplateLine { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsObjectOverloadShouldReturnFalseForNonTemplateLineOrNull()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a.Equals("Same Name"), Is.False);
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void GetHashCodeShouldBeConsistentForEqualLinesBackedByDistinctFieldInstances()
        {
            TemplateLine a = new()
            {
                Coordinate = new([1, 2]),
                MemberBirthName = "Same Name",
                MemberBirthDate = new FamilyDate("1980"),
                InLawBirthName = "Same Spouse",
                InLawBirthDate = new FamilyDate("1981"),
            };
            TemplateLine b = new()
            {
                Coordinate = new([1, 2]),
                MemberBirthName = "Same Name",
                MemberBirthDate = new FamilyDate("1980"),
                InLawBirthName = "Same Spouse",
                InLawBirthDate = new FamilyDate("1981"),
            };

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCodeShouldDifferForDifferentLines()
        {
            TemplateLine a = new() { Coordinate = new([1]), MemberBirthName = "Adams" };
            TemplateLine b = new() { Coordinate = new([2]), MemberBirthName = "Baker" };

            // Hash collisions are possible in theory but extremely unlikely for these values
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        // =====================================================================
        // Comparison operators — both non-null operands
        // =====================================================================

        [Test]
        public void OperatorEqualsShouldReturnTrueForIdenticalLines()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseForDifferentLines()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Adams" };
            TemplateLine? b = new() { Coordinate = new([2]), MemberBirthName = "Baker" };

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnFalseForIdenticalLines()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a != b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueForDifferentLines()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Adams" };
            TemplateLine? b = new() { Coordinate = new([2]), MemberBirthName = "Baker" };

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnTrueWhenFirstIsEarlier()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([2]), MemberBirthName = "Same Name" };

            Assert.That(a < b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenFirstIsLater()
        {
            TemplateLine? a = new() { Coordinate = new([2]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenLinesAreEqual()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenFirstIsEarlier()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([2]), MemberBirthName = "Same Name" };

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenLinesAreEqual()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnFalseWhenFirstIsLater()
        {
            TemplateLine? a = new() { Coordinate = new([2]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a <= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnTrueWhenFirstIsLater()
        {
            TemplateLine? a = new() { Coordinate = new([2]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a > b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenFirstIsEarlier()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([2]), MemberBirthName = "Same Name" };

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenLinesAreEqual()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenFirstIsLater()
        {
            TemplateLine? a = new() { Coordinate = new([2]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenLinesAreEqual()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnFalseWhenFirstIsEarlier()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = new() { Coordinate = new([2]), MemberBirthName = "Same Name" };

            Assert.That(a >= b, Is.False);
        }

        // =====================================================================
        // Comparison operators — null operands
        // =====================================================================

        [Test]
        public void OperatorEqualsShouldReturnTrueForBothNull()
        {
            TemplateLine? a = null;
            TemplateLine? b = null;

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseWhenOnlyLeftIsNull()
        {
            TemplateLine? a = null;
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseWhenOnlyRightIsNull()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = null;

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnFalseForBothNull()
        {
            TemplateLine? a = null;
            TemplateLine? b = null;

            Assert.That(a != b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueWhenOnlyLeftIsNull()
        {
            TemplateLine? a = null;
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueWhenOnlyRightIsNull()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = null;

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseForBothNull()
        {
            TemplateLine? a = null;
            TemplateLine? b = null;

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanShouldReturnTrueWhenLeftIsNullAndRightIsNot()
        {
            TemplateLine? a = null;
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a < b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenRightIsNullAndLeftIsNot()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = null;

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueForBothNull()
        {
            TemplateLine? a = null;
            TemplateLine? b = null;

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenLeftIsNullAndRightIsNot()
        {
            TemplateLine? a = null;
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnFalseWhenRightIsNullAndLeftIsNot()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = null;

            Assert.That(a <= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseForBothNull()
        {
            TemplateLine? a = null;
            TemplateLine? b = null;

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenLeftIsNullAndRightIsNot()
        {
            TemplateLine? a = null;
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnTrueWhenRightIsNullAndLeftIsNot()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = null;

            Assert.That(a > b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueForBothNull()
        {
            TemplateLine? a = null;
            TemplateLine? b = null;

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnFalseWhenLeftIsNullAndRightIsNot()
        {
            TemplateLine? a = null;
            TemplateLine? b = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };

            Assert.That(a >= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenRightIsNullAndLeftIsNot()
        {
            TemplateLine? a = new() { Coordinate = new([1]), MemberBirthName = "Same Name" };
            TemplateLine? b = null;

            Assert.That(a >= b, Is.True);
        }
    }
}
