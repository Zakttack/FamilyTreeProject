using VirtualFamilyMuseumLibrary.Models;
using VirtualFamilyMuseumLibrary.Serialization;
using VirtualFamilyMuseumLibrary.Serialization.Models;

namespace VirtualFamilyMuseumLibraryTest.Models
{
    public class FamilyDateTest
    {
        // =====================================================================
        // CompareTo — Year vs Year (no range)
        // =====================================================================

        [Test]
        public void FirstYearWithOrWithoutEitherMonthOrDayShouldComeBeforeSecondDate()
        {
            FamilyDate firstDate = new("1999");
            FamilyDate secondDate = new("2026");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.LessThan(0));

            FamilyDate thirdDate = new("1999", Month.Jul, 25);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.LessThan(0));
        }

        [Test]
        public void FirstYearWithOrWithoutEitherMonthOrDayShouldComeAfterSecondDate()
        {
            FamilyDate firstDate = new("2026");
            FamilyDate secondDate = new("1999");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.GreaterThan(0));

            FamilyDate thirdDate = new("2026", Month.Jun, 6);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.GreaterThan(0));
        }

        // =====================================================================
        // CompareTo — non-range year vs range year
        // =====================================================================

        [Test]
        public void FirstYearWithOrWithoutEitherMonthOrDayShouldComeBeforeSecondDateConsistingRange()
        {
            FamilyDate firstDate = new("1967");
            FamilyDate secondDate = new("1999-2026");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.LessThan(0));

            FamilyDate thirdDate = new("1967", Month.Oct, 22);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.LessThan(0));
        }

        [Test]
        public void FirstYearWithOrWithoutEitherMonthOrDayShouldComeAfterSecondDateConsistingRange()
        {
            FamilyDate firstDate = new("2026");
            FamilyDate secondDate = new("1967-1999");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.GreaterThan(0));

            FamilyDate thirdDate = new("2026", Month.Jun, 6);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.GreaterThan(0));
        }

        // =====================================================================
        // CompareTo — range year vs non-range year
        // =====================================================================

        [Test]
        public void FirstYearConsistingRangeWithOrWithoutEitherMonthOrDayShouldComeBeforeSecondDate()
        {
            FamilyDate firstDate = new("1967-1999");
            FamilyDate secondDate = new("2026");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.LessThan(0));

            FamilyDate thirdDate = new("1967-1999", Month.May, 31);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.LessThan(0));
        }

        [Test]
        public void FirstYearConsistingRangeWithOrWithoutEitherMonthOrDayShouldComeAfterSecondDate()
        {
            FamilyDate firstDate = new("1999-2026");
            FamilyDate secondDate = new("1967");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.GreaterThan(0));

            FamilyDate thirdDate = new("1967-1999", Month.May, 31);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.GreaterThan(0));
        }

        // =====================================================================
        // CompareTo — range year vs range year
        // =====================================================================

        [Test]
        public void FirstYearConsistingRangeWithOrWithoutEitherMonthOrDayShouldComeBeforeSecondDateConsistingRangeWithoutOverlap()
        {
            FamilyDate firstDate = new("1960-1967");
            FamilyDate secondDate = new("1999-2026");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.LessThan(0));

            FamilyDate thirdDate = new("1960-1967", Month.May, 31);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.LessThan(0));
        }

        [Test]
        public void FirstYearConsistingRangeWithOrWithoutEitherMonthOrDayShouldComeBeforeSecondDateConsistingRangeWithOverlap()
        {
            FamilyDate firstDate = new("1960-1967");
            FamilyDate secondDate = new("1966-2026");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.LessThan(0));

            FamilyDate thirdDate = new("1960-1967", Month.May, 31);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.LessThan(0));
        }

        [Test]
        public void FirstYearConsistingRangeWithOrWithoutEitherMonthOrDayShouldComeAfterSecondDateConsistingRangeWithoutOverlap()
        {
            FamilyDate firstDate = new("1999-2026");
            FamilyDate secondDate = new("1960-1967");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.GreaterThan(0));

            FamilyDate thirdDate = new("1999-2026", Month.May, 31);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.GreaterThan(0));
        }

        [Test]
        public void FirstYearConsistingRangeWithOrWithoutEitherMonthOrDayShouldComeAfterSecondDateConsistingRangeWithOverlap()
        {
            FamilyDate firstDate = new("1966-2026");
            FamilyDate secondDate = new("1960-1967");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.GreaterThan(0));

            FamilyDate thirdDate = new("1966-2026", Month.May, 31);
            int result2 = thirdDate.CompareTo(secondDate);
            Assert.That(result2, Is.GreaterThan(0));
        }

        [Test]
        public void IdenticalRangeYearsShouldCompareAsEqual()
        {
            FamilyDate firstDate = new("1960-1967");
            FamilyDate secondDate = new("1960-1967");

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.EqualTo(0));
        }

        // =====================================================================
        // CompareTo — year exactly matches range boundary
        // =====================================================================

        [Test]
        public void NonRangeYearEqualToRangeMinimumShouldCompareAsEqual()
        {
            FamilyDate firstDate = new("1999");
            FamilyDate secondDate = new("1999-2026");

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void NonRangeYearEqualToRangeMaximumShouldCompareAsEqual()
        {
            FamilyDate firstDate = new("2026");
            FamilyDate secondDate = new("1999-2026");

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void RangeYearWithMinimumEqualToOtherYearShouldCompareAsEqual()
        {
            FamilyDate firstDate = new("1967-1999");
            FamilyDate secondDate = new("1967");

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void RangeYearWithMaximumEqualToOtherYearShouldCompareAsEqual()
        {
            FamilyDate firstDate = new("1967-1999");
            FamilyDate secondDate = new("1999");

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.EqualTo(0));
        }

        // =====================================================================
        // CompareTo — same year, month presence
        // =====================================================================

        [Test]
        public void FirstYearWithNonExistentMonthShouldComeBeforeSecondDateConsistingSameYearButExistentMonth()
        {
            FamilyDate firstDate = new("1960");
            FamilyDate secondDate = new("1960", Month.May);

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.LessThan(0));
        }

        [Test]
        public void FirstYearWithExistentMonthShouldComeAfterSecondDateConsistingSameYearButNonExistentMonth()
        {
            FamilyDate firstDate = new("1960", Month.May);
            FamilyDate secondDate = new("1960");

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.GreaterThan(0));
        }

        [Test]
        public void FirstYearShouldComeBeforeSecondDateConsistingSameYearButDistinctMonths()
        {
            FamilyDate firstDate = new("1960", Month.Jan);
            FamilyDate secondDate = new("1960", Month.May);

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.LessThan(0));
        }

        [Test]
        public void FirstYearShouldComeAfterSecondDateConsistingSameYearButDistinctMonths()
        {
            FamilyDate firstDate = new("1960", Month.May);
            FamilyDate secondDate = new("1960", Month.Feb);

            int result1 = firstDate.CompareTo(secondDate);
            Assert.That(result1, Is.GreaterThan(0));
        }

        [Test]
        public void SameYearAndMonthWithoutDayShouldCompareAsEqual()
        {
            FamilyDate firstDate = new("1960", Month.May);
            FamilyDate secondDate = new("1960", Month.May);

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.EqualTo(0));
        }

        // =====================================================================
        // CompareTo — same year and month, day presence
        // =====================================================================

        [Test]
        public void SameYearAndMonthWithoutDayShouldComeBeforeSameYearAndMonthWithDay()
        {
            FamilyDate firstDate = new("1960", Month.May);
            FamilyDate secondDate = new("1960", Month.May, 10);

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.LessThan(0));
        }

        [Test]
        public void SameYearAndMonthWithDayShouldComeAfterSameYearAndMonthWithoutDay()
        {
            FamilyDate firstDate = new("1960", Month.May, 10);
            FamilyDate secondDate = new("1960", Month.May);

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void SameYearAndMonthEarlierDayShouldComeBeforeLaterDay()
        {
            FamilyDate firstDate = new("1960", Month.May, 3);
            FamilyDate secondDate = new("1960", Month.May, 25);

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.LessThan(0));
        }

        [Test]
        public void SameYearAndMonthLaterDayShouldComeAfterEarlierDay()
        {
            FamilyDate firstDate = new("1960", Month.May, 25);
            FamilyDate secondDate = new("1960", Month.May, 3);

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void IdenticalFullDatesShouldCompareAsEqual()
        {
            FamilyDate firstDate = new("1960", Month.May, 15);
            FamilyDate secondDate = new("1960", Month.May, 15);

            int result = firstDate.CompareTo(secondDate);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void YearOnlyShouldCompareAsEqualToItself()
        {
            FamilyDate date = new("2000");

            int result = date.CompareTo(date);
            Assert.That(result, Is.EqualTo(0));
        }

        // =====================================================================
        // Equals (typed and object overload) + GetHashCode
        // =====================================================================

        [Test]
        public void EqualsDatesWithSameYearOnlyShouldReturnTrue()
        {
            FamilyDate a = new("2000");
            FamilyDate b = new("2000");

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsDatesWithDifferentYearsShouldReturnFalse()
        {
            FamilyDate a = new("2000");
            FamilyDate b = new("2001");

            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void EqualsDatesWithSameYearAndMonthShouldReturnTrue()
        {
            FamilyDate a = new("2000", Month.Mar);
            FamilyDate b = new("2000", Month.Mar);

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsDatesWithSameYearButDifferentMonthsShouldReturnFalse()
        {
            FamilyDate a = new("2000", Month.Mar);
            FamilyDate b = new("2000", Month.Apr);

            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void EqualsDatesWithSameYearMonthAndDayShouldReturnTrue()
        {
            FamilyDate a = new("2000", Month.Mar, 10);
            FamilyDate b = new("2000", Month.Mar, 10);

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsDatesWithSameYearAndMonthButDifferentDaysShouldReturnFalse()
        {
            FamilyDate a = new("2000", Month.Mar, 10);
            FamilyDate b = new("2000", Month.Mar, 11);

            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void EqualsObjectOverloadWithBoxedEqualDateShouldReturnTrue()
        {
            FamilyDate a = new("2000", Month.Mar, 10);
            object b = new FamilyDate("2000", Month.Mar, 10);

            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void EqualsObjectOverloadWithNonFamilyDateShouldReturnFalse()
        {
            FamilyDate a = new("2000");

            Assert.That(a.Equals("2000"), Is.False);
            Assert.That(a.Equals(null), Is.False);
            Assert.That(a.Equals(2000), Is.False);
        }

        [Test]
        public void GetHashCodeShouldBeConsistentForEqualDates()
        {
            FamilyDate a = new("2000", Month.Mar, 10);
            FamilyDate b = new("2000", Month.Mar, 10);

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCodeShouldDifferForDifferentDates()
        {
            FamilyDate a = new("2000", Month.Mar, 10);
            FamilyDate b = new("2001", Month.Apr, 11);

            // Hash collisions are possible in theory but extremely unlikely for these values
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        // =====================================================================
        // ToString
        // =====================================================================

        [Test]
        public void ToStringShouldReturnYearOnlyWhenNoMonthOrDay()
        {
            FamilyDate date = new("1985");
            Assert.That(date.ToString(), Is.EqualTo("1985"));
        }

        [Test]
        public void ToStringShouldReturnMonthAndYearWhenNoDay()
        {
            FamilyDate date = new("1985", Month.Aug);
            Assert.That(date.ToString(), Is.EqualTo("Aug 1985"));
        }

        [Test]
        public void ToStringShouldReturnDayMonthAndYearWhenAllPresent()
        {
            FamilyDate date = new("1985", Month.Aug, 14);
            Assert.That(date.ToString(), Is.EqualTo("14 Aug 1985"));
        }

        [Test]
        public void ToStringShouldReturnRangeYearWhenNoMonthOrDay()
        {
            FamilyDate date = new("1980-1990");
            Assert.That(date.ToString(), Is.EqualTo("1980-1990"));
        }

        [Test]
        public void ToStringShouldReturnMonthAndRangeYearWhenNoDay()
        {
            FamilyDate date = new("1980-1990", Month.Jun);
            Assert.That(date.ToString(), Is.EqualTo("Jun 1980-1990"));
        }

        [Test]
        public void ToStringShouldReturnDayMonthAndRangeYearWhenAllPresent()
        {
            FamilyDate date = new("1980-1990", Month.Jun, 5);
            Assert.That(date.ToString(), Is.EqualTo("5 Jun 1980-1990"));
        }

        // =====================================================================
        // GetDate — valid inputs
        // =====================================================================

        [Test]
        public void GetDateShouldReturnFamilyDateForYearOnlyInput()
        {
            FamilyDate? result = FamilyDate.GetDate("2000");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value.Year, Is.EqualTo("2000"));
            Assert.That(result.Value.Month, Is.Null);
            Assert.That(result.Value.Day, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnFamilyDateForRangeYearOnlyInput()
        {
            FamilyDate? result = FamilyDate.GetDate("1990-2000");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value.Year, Is.EqualTo("1990-2000"));
            Assert.That(result.Value.Month, Is.Null);
            Assert.That(result.Value.Day, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnFamilyDateForMonthAndYearInput()
        {
            FamilyDate? result = FamilyDate.GetDate("Mar 1995");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value.Year, Is.EqualTo("1995"));
            Assert.That(result.Value.Month, Is.EqualTo(Month.Mar));
            Assert.That(result.Value.Day, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnFamilyDateForMonthAndRangeYearInput()
        {
            FamilyDate? result = FamilyDate.GetDate("Jul 1990-2000");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value.Year, Is.EqualTo("1990-2000"));
            Assert.That(result.Value.Month, Is.EqualTo(Month.Jul));
            Assert.That(result.Value.Day, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnFamilyDateForFullDateInput()
        {
            FamilyDate? result = FamilyDate.GetDate("15 Nov 2003");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value.Year, Is.EqualTo("2003"));
            Assert.That(result.Value.Month, Is.EqualTo(Month.Nov));
            Assert.That(result.Value.Day, Is.EqualTo(15));
        }

        [Test]
        public void GetDateShouldReturnFamilyDateForFullDateWithRangeYearInput()
        {
            FamilyDate? result = FamilyDate.GetDate("5 Jun 1980-1990");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value.Year, Is.EqualTo("1980-1990"));
            Assert.That(result.Value.Month, Is.EqualTo(Month.Jun));
            Assert.That(result.Value.Day, Is.EqualTo(5));
        }

        [Test]
        public void GetDateShouldParseAllTwelveMonthAbbreviations()
        {
            string[] months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            Month[] expected = [Month.Jan, Month.Feb, Month.Mar, Month.Apr, Month.May, Month.Jun, Month.Jul, Month.Aug, Month.Sep, Month.Oct, Month.Nov, Month.Dec];

            for (int i = 0; i < months.Length; i++)
            {
                FamilyDate? result = FamilyDate.GetDate($"{months[i]} 2000");
                Assert.That(result, Is.Not.Null, $"Expected non-null for month {months[i]}");
                Assert.That(result!.Value.Month, Is.EqualTo(expected[i]));
            }
        }

        // =====================================================================
        // GetDate — invalid inputs
        // =====================================================================

        [Test]
        public void GetDateShouldReturnNullForEmptyString()
        {
            FamilyDate? result = FamilyDate.GetDate("");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullForTooManyParts()
        {
            FamilyDate? result = FamilyDate.GetDate("15 Nov 2003 extra");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullForInvalidMonthAbbreviation()
        {
            FamilyDate? result = FamilyDate.GetDate("Xyz 2003");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullForInvalidYearFormat()
        {
            FamilyDate? result = FamilyDate.GetDate("999");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullWhenThreePartsButMonthIsInvalid()
        {
            FamilyDate? result = FamilyDate.GetDate("15 Xyz 2003");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullWhenThreePartsButYearIsInvalid()
        {
            FamilyDate? result = FamilyDate.GetDate("15 Nov abc");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullForPlainTextWithNoNumericYear()
        {
            FamilyDate? result = FamilyDate.GetDate("hello");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullForDayTokenWithTrailingNonDigitCharacters()
        {
            FamilyDate? result = null;
            Assert.DoesNotThrow(() => result = FamilyDate.GetDate("15x Nov 2003"));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullForDayTokenWithLeadingNonDigitCharacters()
        {
            FamilyDate? result = null;
            Assert.DoesNotThrow(() => result = FamilyDate.GetDate("x15 Nov 2003"));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullForYearWithExtraTrailingDigit()
        {
            FamilyDate? result = FamilyDate.GetDate("19999");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullForYearWithTrailingLetter()
        {
            FamilyDate? result = FamilyDate.GetDate("1999x");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDateShouldReturnNullForRangeYearWithExtraDigits()
        {
            FamilyDate? result = FamilyDate.GetDate("19990-20260");
            Assert.That(result, Is.Null);
        }

        // =====================================================================
        // Comparison operators — both non-null operands
        // =====================================================================

        [Test]
        public void OperatorEqualsShouldReturnTrueForIdenticalDates()
        {
            FamilyDate? a = new FamilyDate("2000", Month.Jun, 1);
            FamilyDate? b = new FamilyDate("2000", Month.Jun, 1);

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseForDifferentDates()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = new FamilyDate("2001");

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnFalseForIdenticalDates()
        {
            FamilyDate? a = new FamilyDate("2000", Month.Jun, 1);
            FamilyDate? b = new FamilyDate("2000", Month.Jun, 1);

            Assert.That(a != b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueForDifferentDates()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = new FamilyDate("2001");

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnTrueWhenFirstIsEarlier()
        {
            FamilyDate? a = new FamilyDate("1999");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a < b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenFirstIsLater()
        {
            FamilyDate? a = new FamilyDate("2001");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenDatesAreEqual()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenFirstIsEarlier()
        {
            FamilyDate? a = new FamilyDate("1999");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenDatesAreEqual()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnFalseWhenFirstIsLater()
        {
            FamilyDate? a = new FamilyDate("2001");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a <= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnTrueWhenFirstIsLater()
        {
            FamilyDate? a = new FamilyDate("2001");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a > b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenFirstIsEarlier()
        {
            FamilyDate? a = new FamilyDate("1999");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenDatesAreEqual()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenFirstIsLater()
        {
            FamilyDate? a = new FamilyDate("2001");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenDatesAreEqual()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnFalseWhenFirstIsEarlier()
        {
            FamilyDate? a = new FamilyDate("1999");
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a >= b, Is.False);
        }

        // =====================================================================
        // Comparison operators — null operands
        // =====================================================================

        [Test]
        public void OperatorEqualsShouldReturnTrueForBothNull()
        {
            FamilyDate? a = null;
            FamilyDate? b = null;

            Assert.That(a == b, Is.True);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseWhenOnlyLeftIsNull()
        {
            FamilyDate? a = null;
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorEqualsShouldReturnFalseWhenOnlyRightIsNull()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = null;

            Assert.That(a == b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnFalseForBothNull()
        {
            FamilyDate? a = null;
            FamilyDate? b = null;

            Assert.That(a != b, Is.False);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueWhenOnlyLeftIsNull()
        {
            FamilyDate? a = null;
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorNotEqualsShouldReturnTrueWhenOnlyRightIsNull()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = null;

            Assert.That(a != b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseForBothNull()
        {
            FamilyDate? a = null;
            FamilyDate? b = null;

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanShouldReturnTrueWhenLeftIsNullAndRightIsNot()
        {
            FamilyDate? a = null;
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a < b, Is.True);
        }

        [Test]
        public void OperatorLessThanShouldReturnFalseWhenRightIsNullAndLeftIsNot()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = null;

            Assert.That(a < b, Is.False);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueForBothNull()
        {
            FamilyDate? a = null;
            FamilyDate? b = null;

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnTrueWhenLeftIsNullAndRightIsNot()
        {
            FamilyDate? a = null;
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a <= b, Is.True);
        }

        [Test]
        public void OperatorLessThanOrEqualShouldReturnFalseWhenRightIsNullAndLeftIsNot()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = null;

            Assert.That(a <= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseForBothNull()
        {
            FamilyDate? a = null;
            FamilyDate? b = null;

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnFalseWhenLeftIsNullAndRightIsNot()
        {
            FamilyDate? a = null;
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a > b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanShouldReturnTrueWhenRightIsNullAndLeftIsNot()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = null;

            Assert.That(a > b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueForBothNull()
        {
            FamilyDate? a = null;
            FamilyDate? b = null;

            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnFalseWhenLeftIsNullAndRightIsNot()
        {
            FamilyDate? a = null;
            FamilyDate? b = new FamilyDate("2000");

            Assert.That(a >= b, Is.False);
        }

        [Test]
        public void OperatorGreaterThanOrEqualShouldReturnTrueWhenRightIsNullAndLeftIsNot()
        {
            FamilyDate? a = new FamilyDate("2000");
            FamilyDate? b = null;

            Assert.That(a >= b, Is.True);
        }

        // =====================================================================
        // Property accessors
        // =====================================================================

        [Test]
        public void YearPropertyShouldReturnConstructedValue()
        {
            FamilyDate date = new("2024");
            Assert.That(date.Year, Is.EqualTo("2024"));
        }

        [Test]
        public void MonthPropertyShouldReturnNullWhenNotProvided()
        {
            FamilyDate date = new("2024");
            Assert.That(date.Month, Is.Null);
        }

        [Test]
        public void MonthPropertyShouldReturnConstructedValue()
        {
            FamilyDate date = new("2024", Month.Dec);
            Assert.That(date.Month, Is.EqualTo(Month.Dec));
        }

        [Test]
        public void DayPropertyShouldReturnNullWhenNotProvided()
        {
            FamilyDate date = new("2024", Month.Dec);
            Assert.That(date.Day, Is.Null);
        }

        [Test]
        public void DayPropertyShouldReturnConstructedValue()
        {
            FamilyDate date = new("2024", Month.Dec, 31);
            Assert.That(date.Day, Is.EqualTo(31));
        }

        // =====================================================================
        // Value (IBridge)
        // =====================================================================

        [Test]
        public void ValueShouldMatchToStringForYearOnly()
        {
            FamilyDate date = new("1985");
            IBridge bridge = date;
            Assert.That(bridge.Value, Is.EqualTo((BridgeValue)"1985"));
        }

        [Test]
        public void ValueShouldMatchToStringForMonthAndYear()
        {
            FamilyDate date = new("1985", Month.Aug);
            IBridge bridge = date;
            Assert.That(bridge.Value, Is.EqualTo((BridgeValue)"Aug 1985"));
        }

        [Test]
        public void ValueShouldMatchToStringForFullDate()
        {
            FamilyDate date = new("1985", Month.Aug, 14);
            IBridge bridge = date;
            Assert.That(bridge.Value, Is.EqualTo((BridgeValue)"14 Aug 1985"));
        }

        [Test]
        public void ValueShouldRoundTripThroughDeserializeDateForYearOnly()
        {
            FamilyDate original = new("1985");
            IBridge bridge = original;
            Assert.That(bridge.DeserializeDate(), Is.EqualTo(original));
        }

        [Test]
        public void ValueShouldRoundTripThroughDeserializeDateForMonthAndYear()
        {
            FamilyDate original = new("1985", Month.Aug);
            IBridge bridge = original;
            Assert.That(bridge.DeserializeDate(), Is.EqualTo(original));
        }

        [Test]
        public void ValueShouldRoundTripThroughDeserializeDateForFullDate()
        {
            FamilyDate original = new("1985", Month.Aug, 14);
            IBridge bridge = original;
            Assert.That(bridge.DeserializeDate(), Is.EqualTo(original));
        }
    }
}