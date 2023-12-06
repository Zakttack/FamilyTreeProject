namespace FamilyTreeLibrary
{
    public static class HashCodeGenerator<T>
    {
        private static IDictionary<int,T> negativeCollection;
        private static IDictionary<int,T> positiveCollection;

        public static int GenerateHashCode(T value)
        {
            if (value is null)
            {
                return 0;
            }
            negativeCollection ??= new Dictionary<int,T>();
            positiveCollection ??= new Dictionary<int,T>();
            IEnumerable<KeyValuePair<int,T>> negativeMatches = negativeCollection.Where(element => value.Equals(element.Value));
            if (negativeMatches.Any())
            {
                return negativeMatches.First().Key;
            }
            IEnumerable<KeyValuePair<int,T>> positiveMatches = positiveCollection.Where(element => value.Equals(element.Value));
            if (positiveMatches.Any())
            {
                return positiveMatches.First().Key;
            }
            int hashCode;
            do
            {
                hashCode = GenerateInteger();
            } while((hashCode < 0 && negativeCollection.Any(element => element.Key == hashCode))
                || (hashCode > 0 && positiveCollection.Any(element => element.Key == hashCode)));
            if (hashCode < 0)
            {
                negativeCollection.Add(hashCode, value);
            }
            else
            {
                positiveCollection.Add(hashCode, value);
            }
            return hashCode;
        }

        private static int GenerateInteger()
        {
            Random randInt = new();
            int negativeValue = randInt.Next(int.MinValue, 0);
            int positiveValue;
            do
            {
                positiveValue = randInt.Next();
            } while(positiveValue == 0);
            bool resultIsNegative = randInt.Next(2) == 0;
            return resultIsNegative ? negativeValue : ++positiveValue;
        }
    }
}