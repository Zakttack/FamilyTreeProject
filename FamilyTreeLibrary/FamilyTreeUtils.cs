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

        internal static IComparer<Family> Comparer
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

        public static string GetFileNameFromResources(string fileNameWithExtension)
        {
            return GetFileNameFromResources(Directory.GetCurrentDirectory(), fileNameWithExtension);
        }

        public static string WriteDate(DateTime a)
        {
            return $"{a.Month}/{a.Day}/{a.Year}";
        }

        private static string GetFileNameFromResources(string currentPath, string fileNameWithExtension)
        {
            string[] parts = currentPath.Split('\\');
            string current = parts[^1];
            if (current != "FamilyTreeProject")
            {
                return GetFileNameFromResources(currentPath[..(currentPath.Length - current.Length - 1)], fileNameWithExtension);
            }
            return $"{currentPath}\\Resources\\{fileNameWithExtension}";
        }

        private class PersonComparer : IComparer<Family>
        {
            public int Compare(Family a, Family b)
            {
                return CompareDates(a.Couple[0].BirthDate, b.Couple[0].BirthDate);
            }
        }
    }
}
