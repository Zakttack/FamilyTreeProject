using FamilyTreeLibrary.Exceptions;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary.Models
{
    public struct FamilyTreeDate : IComparable<FamilyTreeDate>, IEquatable<FamilyTreeDate>
    {
        private int day;
        private string month;
        private string year;
        private IReadOnlyDictionary<string,int> months;
        public FamilyTreeDate(){}
        public FamilyTreeDate(string input)
        {
            month = FamilyTreeUtils.DefaultDate.Month;
            day = FamilyTreeUtils.DefaultDate.Day;
            year = FamilyTreeUtils.DefaultDate.Year;
            if (input is not null & input != string.Empty & input.Length < 4)
            {
                string[] values = input.Split(' ');
                foreach (string value in values)
                {
                    if (value.Length < 3)
                    {
                        day = value == "" ? 0 : Convert.ToInt32(value);
                    }
                    else if (value.Length == 3)
                    {
                        month = value;
                    }
                    else
                    {
                        year = value;
                    }
                }
            }
            Year = year;
            Month = month;
            Day = day;
        }

        public int Day
        {
            readonly get
            {
                return day;
            }
            set
            {
                if (value < 0 || value > months[Month])
                {
                    throw new InvalidDateException(value, DateAttributes.Day);
                }
                day = value;
            }
        }

        public string Month
        {
            readonly get
            {
                return month;
            }
            set
            {
                if(!Months.ContainsKey(value))
                {
                    throw new InvalidDateException(value, DateAttributes.Month);
                }
                month = value;
            }
        }

        public string Year
        {
            readonly get
            {
                return year;
            }
            set
            {
                bool isNumber = Regex.IsMatch(value, FamilyTreeUtils.NUMBER_PATTERN);
                if (value != "" && !isNumber && !Regex.IsMatch(value, FamilyTreeUtils.RANGE_PATTERN))
                {
                    throw new InvalidDateException(value, DateAttributes.Year);
                }
                year = value;
                FillMonths(isNumber && DateTime.IsLeapYear(Convert.ToInt32(year)));
            }
        }

        public readonly int CompareTo(FamilyTreeDate other)
        {
            int yearDiff = Year.CompareTo(other.Year);
            if (yearDiff != 0)
            {
                return yearDiff;
            }
            int monthDiff = Math.Abs(months.Keys.ToList().IndexOf(Month) - months.Keys.ToList().IndexOf(other.Month));
            return monthDiff != 0 ? monthDiff : Math.Abs(Day - other.Day);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is FamilyTreeDate other && Equals(other);
        }

        public readonly bool Equals(FamilyTreeDate other)
        {
            return CompareTo(other) == 0;
        }

        public override readonly int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override readonly string ToString()
        {
            string output = "";
            if (Day > 0)
            {
                output += Day;
            }
            if (Month != "")
            {
                output += $" {Month} ";
            }
            if (Year != "")
            {
                output += Year;
            }
            return this == FamilyTreeUtils.DefaultDate ? null : output.Trim();
        }

        public static bool operator==(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = dateA == default;
            bool dateBIsDefault = dateB == default;
            return (dateAIsDefault && dateBIsDefault) || (!dateAIsDefault && dateA.Equals(dateB));
        }

        public static bool operator!=(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = dateA == default;
            bool dateBIsDefault = dateB == default;
            return (!dateAIsDefault || !dateBIsDefault) && (dateAIsDefault || !dateA.Equals(dateB));
        }

        public static bool operator<(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = dateA == default;
            bool dateBIsDefault = dateB == default;
            return (dateAIsDefault && !dateBIsDefault) || (!dateBIsDefault && dateA.CompareTo(dateB) < 0);
        }

        public static bool operator<=(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = dateA == default;
            bool dateBIsDefault = dateB == default;
            return dateAIsDefault || (!dateBIsDefault && dateA.CompareTo(dateB) <= 0);
        }

        public static bool operator>(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = dateA == default;
            bool dateBIsDefault = dateB == default;
            return (!dateAIsDefault && dateBIsDefault) || (!dateAIsDefault && dateA.CompareTo(dateB) > 0);
        }

        public static bool operator>=(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            bool dateAIsDefault = dateA == default;
            bool dateBIsDefault = dateB == default;
            return dateBIsDefault || (!dateAIsDefault && dateA.CompareTo(dateB) <= 0);
        }

        internal readonly IReadOnlyDictionary<string,int> Months
        {
            get
            {
                return months;
            }
        }

        private void FillMonths(bool isLeapYear)
        {
            months = new Dictionary<string,int>
                {
                    {"", 0},
                    {"Jan", 31},
                    {"Feb",isLeapYear ? 29 : 28},
                    {"Mar",31},
                    {"Apr",30},
                    {"May",31},
                    {"Jun",30},
                    {"Jul",31},
                    {"Aug",31},
                    {"Sep",30},
                    {"Oct",31},
                    {"Nov",30},
                    {"Dec",31}
                };
        }
    }
}