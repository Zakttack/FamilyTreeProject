using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
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

        public static IComparer<AbstractOrderingType[]> OrderTypeComparer
        {
            get
            {
                return new OrderingTypeComparer();
            }
        }

        public static string GetFileNameFromResources(string fileNameWithExtension)
        {
            return GetFileNameFromResources(Directory.GetCurrentDirectory(), fileNameWithExtension);
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

        private class OrderingTypeComparer : IComparer<AbstractOrderingType[]>
        {
            public int Compare(AbstractOrderingType[] a, AbstractOrderingType[] b)
            {
                for (int i = 0; i < a.Length && i < b.Length; i++)
                {
                    if (a[i].CompareTo(b[i]) < 0)
                    {
                        return -1;
                    }
                    else if (a[i].CompareTo(b[i]) > 0)
                    {
                        return 1;
                    }
                }
                return a.Length - b.Length;
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
