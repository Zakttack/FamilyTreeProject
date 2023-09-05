using FamilyTreeLibrary.Models;
using Newtonsoft.Json;
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

    internal static IComparer<Person> Comparer
    {
        get
        {
            return new PersonComparer();
        }
    }

    public static int CompareDates(DateTime a, DateTime b)
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

    public static string WriteDate(DateTime a)
    {
        return $"{a.Month}/{a.Day}/{a.Year}";
    }

    private class PersonComparer : IComparer<Person>
    {
        public int Compare(Person a, Person b)
        {
            return CompareDates(a.BirthDate, b.BirthDate);
        }
    }
}
}
