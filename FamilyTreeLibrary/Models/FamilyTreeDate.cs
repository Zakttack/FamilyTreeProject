using System.Diagnostics.CodeAnalysis;
using FamilyTreeLibrary.Exceptions;

namespace FamilyTreeLibrary.Models
{
    public struct FamilyTreeDate : IComparable<FamilyTreeDate>
    {
        private int day;
        private string month;
        private int year;
        public FamilyTreeDate(int day = 0, string month = "", int year = 0)
        {
            Day = day;
            Month = month;
            Year = year;
        }

        public FamilyTreeDate(string input)
        {
            string[] values = input.Split(' ');
            switch (values.Length)
            {
                case 0: Day = 0; Month = ""; Year = 0; break;
                case 1:
                    switch (input.Length)
                    {
                        case 2: Day = Convert.ToInt32(input); Month = ""; Year = 0; break;
                        case 3: Day = 0; Month = input; Year = 0; break;
                        case 4: Day = 0;
                            if (int.TryParse(input, out int value))
                            {
                                Month = "";
                                Year = value;
                            }
                            else
                            {
                                Month = input;
                                Year = 0;
                            }
                        break;
                        default: throw new InvalidDateException(input);
                    }
                break;
                case 2:
                    bool foundDay = int.TryParse(values[0], out int day);
                    bool foundYear = int.TryParse(values[1], out int year);
                    if (foundDay & foundYear)
                    {
                        Day = day; Month = ""; Year = year;
                    }
                    else if (foundDay & !foundYear)
                    {
                        Day = day; Month = values[1]; Year = 0;
                    }
                    else if (!foundDay & foundYear)
                    {
                        Day = 0; Month = values[0]; Year = year;
                    }
                    else
                    {
                        throw new InvalidDateException(input);
                    }
                break;
                case 3: Day = Convert.ToInt32(values[0]); Month = values[1]; Year = Convert.ToInt32(values[2]); break;
                default: throw new InvalidDateException(input);
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
                if (value == "June")
                {
                    month = "Jun";
                }
                else if(!Months.Contains(value))
                {
                    throw new InvalidDateException(value, DateAttributes.Month);
                }
                else
                {
                    month = value;
                }
            }
        }

        public int Year
        {
            readonly get
            {
                return year;
            }
            set
            {
                if (value < 0)
                {
                    throw new InvalidDateException(value, DateAttributes.Year);
                }
                year = value;
            }
        }

        public readonly int CompareTo(FamilyTreeDate other)
        {
            int yearDiff = Math.Abs(Year - other.Year);
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
            if (Year > 0)
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