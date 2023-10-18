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
            string digitPattern = @"^\d+$";
            switch (values.Length)
            {
                case 1: 
                    if (IsMonth(input))
                    {
                        return Convert.ToDateTime($"01 {input} 0001");
                    }
                    if (Regex.IsMatch(input, digitPattern))
                    {
                        switch (input.Length)
                        {
                            case 2: return Convert.ToDateTime($"{input} Jan 0001");
                            case 4: return Convert.ToDateTime($"01 Jan {input}");
                        }
                    }
                break;
                case 2:
                    if (IsMonth(values[0]))
                    {
                        return Convert.ToDateTime($"01 {values[0]} {values[1]}");
                    }
                    else if (Regex.IsMatch(values[0], digitPattern) && values[0].Length == 2 && IsMonth(values[1]))
                    {
                        return Convert.ToDateTime($"{values[0]} {values[1]} 0001");
                    }
                break;
                case 3: return Convert.ToDateTime(input);
            }
            return default;
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

        public static bool MemberEquivalent(Family main, Family duplicate)
        {
            string[] memberMainNameParts = main.Member.Name.Split(' ');
            string[] memberDuplicateNameParts = duplicate.Member.Name.Split(' ');
            int index = -1;
            for (int i = 0; i < Math.Min(memberMainNameParts.Length, memberDuplicateNameParts.Length) - 1 && index < 0; i++)
            {
                if (memberMainNameParts[i] == memberDuplicateNameParts[i])
                {
                    index = i;
                }
            }
            if (index < 0 || index > Math.Min(memberMainNameParts.Length, memberDuplicateNameParts.Length) - 2)
            {
                return false;
            }
            for (int i1 = index + 1; i1 < memberDuplicateNameParts.Length; i1++)
            {
                if (memberDuplicateNameParts[i1] == memberMainNameParts[^1])
                {
                    return true;
                }
            }
            return false;
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
                string pattern = @"^[0-9]+\-[0-9]+$";
                StringBuilder tokenRebuilder = new();
                if (Regex.IsMatch(token, pattern))
                {
                    string[] values = token.Split('-');
                    int v1 = Convert.ToInt32(values[0]);
                    int v2;
                    if (values[0].Length == values[1].Length)
                    {
                        v2 = Convert.ToInt32(values[1]);
                    }
                    else
                    {
                        int stop = values[0].Length - values[1].Length;
                        for (int i = 0; i < values[0].Length; i++)
                        {
                            tokenRebuilder.Append(i < stop ? values[0][i] : values[1][i - stop]);
                        }
                        v2 = Convert.ToInt32(tokenRebuilder.ToString());
                    }
                    return $"{(v1 + v2) / 2}";
                }
                string pattern1 = @"^[^a-zA-Z0-9]$";
                if (Regex.IsMatch(token, pattern1))
                {
                    return "";
                }
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