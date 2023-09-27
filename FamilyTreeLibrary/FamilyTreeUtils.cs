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

        public static AbstractOrderingType[] IncrementGeneration(AbstractOrderingType[] temp)
        {
            IList<AbstractOrderingType> collection = temp.ToList();
            collection.Add(AbstractOrderingType.GetOrderingType(1, temp.Length + 1));
            return collection.ToArray();
        }

        public static string[] GetSubCollection(string[] collection, int start, int end)
        {
            return collection.AsSpan(start, end - start + 1).ToArray();
        }

        public static string[] GetSubCollection(string[] collection, int start)
        {
            return collection.AsSpan(start).ToArray();
        }

        public static AbstractOrderingType[] ReplaceWithIncrementByKey(AbstractOrderingType[] temp)
        {
            AbstractOrderingType[] collection = new AbstractOrderingType[temp.Length];
            Array.Copy(temp, collection, temp.Length - 1);
            collection[^1] = AbstractOrderingType.GetOrderingType(temp[^1].ConversionPair.Key + 1, temp.Length);
            return collection;
        }

        public static IList<string> SubTokenCollection(IList<string> tokens, int start, int end)
        {
            return tokens.Skip(start).Take(end - start + 1).ToList();
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