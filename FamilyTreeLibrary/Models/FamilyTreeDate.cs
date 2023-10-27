using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using FamilyTreeLibrary.Exceptions;

namespace FamilyTreeLibrary.Models
{
    public struct FamilyTreeDate : IComparable<FamilyTreeDate>
    {
        private int day;
        private string month;
        private string year;
        public FamilyTreeDate(int day = 0, string month = "", string year = "")
        {
            Day = day;
            Month = month;
            Year = year;
        }

        public FamilyTreeDate(string input)
        {
            string[] values = input.Split(' ');
            Day = 0; Month = ""; Year = "";
            foreach (string value in values)
            {
                if (value.Length < 3)
                {
                    Day = Convert.ToInt32(value);
                }
                else if (value.Length == 3)
                {
                    Month = value;
                }
                else
                {
                    Year = value;
                }
            }
        }

        public int Day
        {
            readonly get
            {
                return day;
            }
            set
            {
                if (value < 0)
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
                if(!Months.Contains(value))
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
                if (Regex.IsMatch(value, FamilyTreeUtils.NUMBER_PATTERN))
                {
                    year = value;
                }
                else
                {
                    if (Regex.IsMatch(value, FamilyTreeUtils.RANGE_PATTERN))
                    {
                        year = value;
                    }
                    else
                    {
                        throw new InvalidDateException(value, DateAttributes.Year);
                    }
                }
            }
        }

        public readonly int CompareTo(FamilyTreeDate other)
        {
            int yearDiff = Year.CompareTo(other.Year);
            if (yearDiff != 0)
            {
                return yearDiff;
            }
            int monthDiff = Math.Abs(Months.ToList().IndexOf(Month) - Months.ToList().IndexOf(other.Month));
            if (monthDiff != 0)
            {
                return monthDiff;
            }
            int dayDiff = Math.Abs(Day - other.Day);
            return dayDiff != 0 ? dayDiff : 0;
        }

        public override readonly bool Equals([NotNullWhen(true)] object obj)
        {
            return ToString() == obj.ToString();
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
            return output.Trim();
        }

        public static bool operator==(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            return dateA.Equals(dateB);
        }

        public static bool operator!=(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            return !dateA.Equals(dateB);
        }

        public static bool operator<(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            return dateA.CompareTo(dateB) < 0;
        }

        public static bool operator<=(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            return dateA.CompareTo(dateB) <= 0;
        }

        public static bool operator>(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            return dateA.CompareTo(dateB) > 0;
        }

        public static bool operator>=(FamilyTreeDate dateA, FamilyTreeDate dateB)
        {
            return dateA.CompareTo(dateB) <= 0;
        }

        internal static IReadOnlySet<string> Months
        {
            get
            {
                return new HashSet<string>
                {
                    "",
                    "Jan",
                    "Feb",
                    "Mar",
                    "Apr",
                    "May",
                    "Jun",
                    "Jul",
                    "Aug",
                    "Sep",
                    "Oct",
                    "Nov",
                    "Dec"
                };
            }
        }
    }
}