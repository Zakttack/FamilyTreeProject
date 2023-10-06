using System.Text;
using System.Text.RegularExpressions;
using FamilyTreeLibrary.Comparers;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
namespace FamilyTreeLibrary
{
    public static class FamilyTreeUtils
    {
        public static IComparer<DateTime> ComparerDate
        {
            get
            {
                return new DateComparer();
            }
        }

        public static IComparer<Family> ComparerFamily
        {
            get
            {
                return new FamilyComparer();
            }
        }
        public static string GetFileNameFromResources(string currentPath, string fileNameWithExtension)
        {
            string[] parts = currentPath.Split('\\');
            string current = parts[^1];
            if (current != "FamilyTreeProject")
            {
                return GetFileNameFromResources(currentPath[..(currentPath.Length - current.Length - 1)], fileNameWithExtension);
            }
            return $"{currentPath}\\Resources\\{fileNameWithExtension}";
        }

        public static bool IsMonth(string token)
        {
            IReadOnlySet<string> months = new HashSet<string>
            {
                "Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "June",
                "Jul",
                "Aug",
                "Sep",
                "Oct",
                "Nov",
                "Dec"
            };
            return months.Contains(token);
        }

        public static AbstractOrderingType[] IncrementGeneration(AbstractOrderingType[] temp)
        {
            IList<AbstractOrderingType> collection = temp.ToList();
            collection.Add(AbstractOrderingType.GetOrderingType(1, temp.Length + 1));
            return collection.ToArray();
        }

        public static AbstractOrderingType GetOrderingTypeByLine(string line)
        {
            string token = line.Split(' ')[0];
            for (int generation = 1; generation <= 6; generation++)
            {
                if (AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, token, generation))
                {
                    return orderingType;
                }
            }
            return null;
        }
        public static string ReformatToken(string token)
        {
            string pattern = "^[A-Z][a-z]+\\d+$";
            if (Regex.IsMatch(token, pattern))
            {
                StringBuilder newTokenBuilder = new();
                foreach (char c in token)
                {
                    if (char.IsDigit(c))
                    {
                        newTokenBuilder.Append(' ');
                    }
                    newTokenBuilder.Append(token);
                }
                return newTokenBuilder.ToString();
            }
            return token;
        }

        public static AbstractOrderingType[] ReplaceWithIncrementByKey(AbstractOrderingType[] temp)
        {
            AbstractOrderingType[] collection = new AbstractOrderingType[temp.Length];
            Array.Copy(temp, collection, temp.Length - 1);
            collection[^1] = AbstractOrderingType.GetOrderingType(temp[^1].ConversionPair.Key + 1, temp.Length);
            return collection;
        }
    }
}