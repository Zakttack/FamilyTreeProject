﻿using FamilyTreeLibrary.Models;
namespace FamilyTreeLibrary
{
    public class FamilyTreeUtils
    {
        public static DateTime DefaultDate
        {
            get
            {
                return new();
            }
        }

        public static IComparer<DateTime> DateComp
        {
            get
            {
                return new DateComparer();
            }
        }

        public static IComparer<Family> FamilyComparer
        {
            get
            {
                return new PersonComparer();
            }
        }

        public static string[] GetSubCollection(string[] collection, int start, int end)
        {
            return collection.AsSpan(start, end - start + 1).ToArray();
        }

        public static string[] GetSubCollection(string[] collection, int start)
        {
            return collection.AsSpan(start).ToArray();
        }

        private class DateComparer : IComparer<DateTime>
        {
            public int Compare(DateTime a, DateTime b)
            {
                int yearDiff = a.Year - b.Year;
                int monthDiff = a.Month - b.Month;
                int dayDiff = a.Day - b.Day;
                if (yearDiff == 0)
                {
                    if (monthDiff == 0)
                    {
                        return dayDiff;
                    }
                    return monthDiff;
                }
                return yearDiff;
            }
        }

        private class PersonComparer : IComparer<Family>
        {
            public int Compare(Family a, Family b)
            {
                return DateComp.Compare(a.Couple[0].BirthDate, b.Couple[0].BirthDate);
            }
        }
    }
}