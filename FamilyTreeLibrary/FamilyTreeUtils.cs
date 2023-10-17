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

        public static Family Root
        {
            get
            {
                string name = default;
                return new(new Person(name));
            }
        }

        public static AbstractOrderingType[] CopyOrderingType(AbstractOrderingType[] temp)
        {
            AbstractOrderingType[] collection = new AbstractOrderingType[temp.Length];
            Array.Copy(temp, collection, temp.Length);
            return collection;
        }

        public static DateTime GetDate(string input)
        {
            string[] values = input.Split(' ');
            StringBuilder builder = new();
            if (values.Length < 3)
            {
                if (IsMonth(values[0]))
                {
                    builder.Append("01");
                    foreach (string value in values)
                    {
                        builder.Append($" {value}");
                    }
                }
                else if (IsMonth(values[1]))
                {
                    foreach (string value in values)
                    {
                        builder.Append($"{value} ");
                    }
                    builder.Append("0001");
                }
                return Convert.ToDateTime(builder.ToString());
            }
            return Convert.ToDateTime(input);
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

        public static Queue<AbstractOrderingType> GetOrderingTypeByLine(string line)
        {
            Queue<AbstractOrderingType> result = new();
            string token = line.Split(' ')[0];
            for (int generation = 1; generation <= 6; generation++)
            {
                if (AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, token, generation))
                {
                    result.Enqueue(orderingType);
                }
            }
            return result;
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

        public static AbstractOrderingType[] NextOrderingType(AbstractOrderingType[] temp, AbstractOrderingType orderingType)
        {
            if (temp.Length == 0)
            {
                return IncrementGeneration(new AbstractOrderingType[0]);
            }
            List<AbstractOrderingType[]> possibleNexts = new()
            {
                IncrementGeneration(temp),
                ReplaceWithIncrementByKey(temp)
            };
            AbstractOrderingType[] previous = temp;
            while (true)
            {
                previous = PreviousOrderingType(previous);
                if (previous.Length == 0)
                {
                    break;
                }
                possibleNexts.Add(ReplaceWithIncrementByKey(previous));
            }
            foreach (AbstractOrderingType[] value in possibleNexts)
            {
                if (value[^1].Equals(orderingType))
                {
                    return value;
                }
            }
            return null;
        }

        public static AbstractOrderingType[] PreviousOrderingType(AbstractOrderingType[] current)
        {
            AbstractOrderingType[] previous = new AbstractOrderingType[current.Length - 1];
            Array.Copy(current, previous, current.Length - 1);
            return previous;
        }
        public static string ReformatToken(string token)
        {
            if (token.Length > 0)
            {
                StringBuilder tokenRebuilder = new();
                for (int i = 0; i < token.Length - 1; i++)
                {
                    tokenRebuilder.Append(token[i]);
                    if (char.IsDigit(token[i]) ^ char.IsDigit(token[i+1]))
                    {
                        tokenRebuilder.Append(' ');
                    }
                }
                tokenRebuilder.Append(token[^1]);
                return tokenRebuilder.ToString();
            }
            return token;
        }

        private static AbstractOrderingType[] IncrementGeneration(AbstractOrderingType[] temp)
        {
            IList<AbstractOrderingType> collection = temp.ToList();
            collection.Add(AbstractOrderingType.GetOrderingType(1, temp.Length + 1));
            return collection.ToArray();
        }

        private static AbstractOrderingType[] ReplaceWithIncrementByKey(AbstractOrderingType[] temp)
        {
            AbstractOrderingType[] collection = new AbstractOrderingType[temp.Length];
            if (temp.Length == 1)
            {
                collection[0] = AbstractOrderingType.GetOrderingType(temp[0].ConversionPair.Key + 1, temp.Length);
            }
            else
            {
                Array.Copy(temp, collection, temp.Length - 1);
                collection[^1] = AbstractOrderingType.GetOrderingType(temp[^1].ConversionPair.Key + 1, temp.Length);
            }
            return collection;
        }
    }
}