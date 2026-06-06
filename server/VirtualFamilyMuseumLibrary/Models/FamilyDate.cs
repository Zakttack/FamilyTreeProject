using System.Text;
using System.Text.RegularExpressions;

namespace VirtualFamilyMuseumLibrary.Models
{
    public readonly partial struct FamilyDate(string year, Month? month = null, int? day = null) : IComparable<FamilyDate>, IEquatable<FamilyDate>
    {
        public string Year
        {
            get
            {
                return year;
            }
        }

        public Month? Month
        {
            get
            {
                return month;
            }
        }

        public int? Day
        {
            get
            {
                return day;
            }
        }

        public int CompareTo(FamilyDate other)
        {
            bool yearIsRange = YearRangeRegex().IsMatch(year);
            bool otherYearIsRange = YearRangeRegex().IsMatch(other.Year);
            if (!yearIsRange && !otherYearIsRange)
            {
                int yearCompare = year.CompareTo(other.Year);
                if (yearCompare != 0)
                {
                    return yearCompare;
                }
            }
            else if (!yearIsRange && otherYearIsRange)
            {
                SortedSet<string> otherYearParts = [.. other.Year.Split('-')];
                if (year.CompareTo(otherYearParts.Min) < 0)
                {
                    return -1;
                }
                else if (year.CompareTo(otherYearParts.Max) > 0)
                {
                    return 1;
                }
            }
            else if (yearIsRange && !otherYearIsRange)
            {
                SortedSet<string> yearParts = [.. year.Split('-')];
                if (yearParts.Max!.CompareTo(other.Year) < 0)
                {
                    return -1;
                }
                else if (yearParts.Min!.CompareTo(other.Year) > 0)
                {
                    return 1;
                }
            }
            else if (yearIsRange && otherYearIsRange)
            {
                int yearCompare = year.CompareTo(other.Year);
                if (yearCompare != 0)
                {
                    return yearCompare;
                }
            }
            if (month is null && other.Month is not null)
            {
                return -1;
            }
            else if (month is not null && other.Month is null)
            {
                return 1;
            }
            else if (month is not null && other.Month is not null)
            {
                int monthCompare = month.Value.CompareTo(other.Month);
                if (monthCompare != 0)
                {
                    return monthCompare;
                }
            }
            if (day is null && other.Day is not null)
            {
                return -1;
            }
            else if (day is not null && other.Day is null)
            {
                return 1;
            }
            else if (day is not null && other.Day is not null)
            {
                int dayCompare = day.Value - other.Day.Value;
                if (dayCompare != 0)
                {
                    return dayCompare;
                }
            }
            return 0;
        }

        public bool Equals(FamilyDate other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is FamilyDate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(day, month, year);
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            if (day is not null)
            {
                builder.Append($"{day} ");
            }
            if (month is not null)
            {
                builder.Append($"{month} ");
            }
            builder.Append(year);
            return builder.ToString();
        }

        public static FamilyDate? GetDate(string input)
        {
            string[] parts = input.Split();
            if (parts.Length < 1 || parts.Length > 3)
            {
                return null;
            }
            else if (parts.Length == 3 && DayRegex().IsMatch(parts[0]))
            {
                int day = Convert.ToInt32(parts[0]);
                Month? month = GetMonth(parts[1]);
                if (month is null)
                {
                    return null;
                }
                return YearNonRangeRegex().IsMatch(parts[2]) || YearRangeRegex().IsMatch(parts[2]) ? new FamilyDate(parts[2], month, day) : null;
            }
            Month? month1 = GetMonth(parts[0]);
            if (parts.Length == 2 && month1 is not null)
            {
                return YearNonRangeRegex().IsMatch(parts[1]) || YearRangeRegex().IsMatch(parts[1]) ? new FamilyDate(parts[1], month1) : null;
            }
            return parts.Length == 1 && (YearNonRangeRegex().IsMatch(parts[0]) || YearRangeRegex().IsMatch(parts[0])) ? new FamilyDate(parts[0]) : null;
        }

        public static bool operator!=(FamilyDate? a, FamilyDate? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            else if (a is null && b is not null)
            {
                return true;
            }
            else if (a is not null && b is null)
            {
                return true;
            }
            return !a!.Value.Equals(b!.Value);
        }

        public static bool operator<(FamilyDate? a, FamilyDate? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            else if (a is null && b is not null)
            {
                return true;
            }
            else if (a is not null && b is null)
            {
                return false;
            }
            return a!.Value.CompareTo(b!.Value) < 0;
        }

        public static bool operator<=(FamilyDate? a, FamilyDate? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null && b is not null)
            {
                return true;
            }
            else if (a is not null && b is null)
            {
                return false;
            }
            return a!.Value.CompareTo(b!.Value) <= 0;
        }

        public static bool operator==(FamilyDate? a, FamilyDate? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null && b is not null)
            {
                return false;
            }
            else if (a is not null && b is null)
            {
                return false;
            }
            return a!.Value.Equals(b!.Value);
        }

        public static bool operator>=(FamilyDate? a, FamilyDate? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            else if (a is null && b is not null)
            {
                return false;
            }
            else if (a is not null && b is null)
            {
                return true;
            }
            return a!.Value.CompareTo(b!.Value) >= 0;
        }

        public static bool operator>(FamilyDate? a, FamilyDate? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            else if (a is null && b is not null)
            {
                return false;
            }
            else if (a is not null && b is null)
            {
                return true;
            }
            return a!.Value.CompareTo(b!.Value) > 0;
        }

        private static Month? GetMonth(string input)
        {
            return input switch
            {
                "Jan" => (Month?)Models.Month.Jan,
                "Feb" => (Month?)Models.Month.Feb,
                "Mar" => (Month?)Models.Month.Mar,
                "Apr" => (Month?)Models.Month.Apr,
                "May" => (Month?)Models.Month.May,
                "Jun" => (Month?)Models.Month.Jun,
                "Jul" => (Month?)Models.Month.Jul,
                "Aug" => (Month?)Models.Month.Aug,
                "Sep" => (Month?)Models.Month.Sep,
                "Oct" => (Month?)Models.Month.Oct,
                "Nov" => (Month?)Models.Month.Nov,
                "Dec" => (Month?)Models.Month.Dec,
                _ => null
            };
        }

        [GeneratedRegex(@"\d+")]
        private static partial Regex DayRegex();

        [GeneratedRegex(@"\d\d\d\d")]
        private static partial Regex YearNonRangeRegex();

        [GeneratedRegex(@"\d\d\d\d-\d\d\d\d")]
        private static partial Regex YearRangeRegex();
    }
}