using System.Text;
using System.Text.RegularExpressions;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;
using Serilog;
namespace FamilyTreeLibrary
{
    public static class FamilyTreeUtils
    {
        internal const string NUMBER_PATTERN = @"^\d+$";
        internal const string RANGE_PATTERN = @"^\d+-\d+$";
        public static Family Root
        {
            get
            {
                string value = default;
                return new(new Person(value));
            }
        }

        public static ILogger Logger
        {
            get
            {
                string filePath = GetFileNameFromResources(Directory.GetCurrentDirectory(), "log.txt");
                return new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(filePath)
                    .CreateLogger();
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
            return $@"{currentPath}\Resources\{fileNameWithExtension}";
        }

        public static Queue<AbstractOrderingType> GetOrderingTypeByLine(string line, int maxKey = int.MaxValue)
        {
            Queue<AbstractOrderingType> result = new();
            string token = line.Split(' ')[0];
            for (int generation = 1; generation <= 6; generation++)
            {
                if (AbstractOrderingType.TryGetOrderingType(out AbstractOrderingType orderingType, token, generation, maxKey))
                {
                    result.Enqueue(orderingType);
                }
            }
            return result;
        }

        public static AbstractOrderingType[] NextOrderingType(AbstractOrderingType[] temp, AbstractOrderingType orderingType)
        {
            if (temp.Length == 0)
            {
                return IncrementGeneration(Array.Empty<AbstractOrderingType>());
            }
            ICollection<AbstractOrderingType[]> possibleNexts = new List<AbstractOrderingType[]>();
            if (temp.Length < 6)
            {
                possibleNexts.Add(IncrementGeneration(temp));
            }
            possibleNexts.Add(ReplaceWithIncrementByKey(temp));
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
                string specialCharacterPattern = "[^a-zA-Z0-9,.-]";
                if (Regex.IsMatch(token, specialCharacterPattern))
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