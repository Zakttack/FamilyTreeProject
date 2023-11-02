namespace FamilyTreeLibrary.OrderingType
{
    public abstract class AbstractOrderingType : IComparable<AbstractOrderingType>
    {
        protected AbstractOrderingType(int key, int generation, int maxKey)
        {
            Generation = generation;
            MaxKey = maxKey;
            ConversionPair = new(key, FindValue(key));
        }

        protected AbstractOrderingType(string value, int generation, int maxKey)
        {
            Generation = generation;
            MaxKey = maxKey;
            ConversionPair = new(FindKey(value), value);
        }

        public KeyValuePair<int,string> ConversionPair
        {
            get;
        }

        protected OrderingTypeTypes Type
        {
            get
            {
                return Generation switch
                {
                    1 => OrderingTypeTypes.RomanNumeralUpper,
                    2 => OrderingTypeTypes.CapitalLetter,
                    3 => OrderingTypeTypes.Numbering,
                    4 => OrderingTypeTypes.LowerCaseLetter,
                    5 => OrderingTypeTypes.ParenthesizedNumbering,
                    6 => OrderingTypeTypes.RomanNumeralLower,
                    _ => throw new NotSupportedException($"{Generation} isn't supported.")
                };
            }
        }

        public int CompareTo(AbstractOrderingType other)
        {
            int temp = Type.CompareTo(other.Type);
            return temp == 0 ? ConversionPair.Key - other.ConversionPair.Key : temp;
        }

        public override bool Equals(object obj)
        {
            return obj != null && ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ConversionPair.GetHashCode();
        }

        public static AbstractOrderingType GetOrderingType(int key, int generation, int maxKey = int.MaxValue)
        {
            return generation switch
            {
                1 => new RomanNumeralOrderingType(key, true, maxKey),
                2 => new LetterOrderingType(key, true, maxKey),
                3 => new DigitOrderingType(key, false, maxKey),
                4 => new LetterOrderingType(key, false, maxKey),
                5 => new ParenthesizedDigitOrderingType(key, maxKey),
                6 => new RomanNumeralOrderingType(key, false, maxKey),
                _ => null
            };
        }

        public override string ToString()
        {
            return ConversionPair.ToString();
        }

        public static bool TryGetOrderingType(out AbstractOrderingType orderingType, string value, int generation, int maxKey = int.MaxValue)
        {
            switch (generation)
            {
                case 1: orderingType = new RomanNumeralOrderingType(value, true, maxKey); break;
                case 2: orderingType = new LetterOrderingType(value, true, maxKey); break;
                case 3: orderingType = new DigitOrderingType(value, false, maxKey); break;
                case 4: orderingType = new LetterOrderingType(value, false, maxKey); break;
                case 5: orderingType = new ParenthesizedDigitOrderingType(value, maxKey); break;
                case 6: orderingType = new RomanNumeralOrderingType(value, false, maxKey); break;
                default: orderingType = null; return false; 
            }
            return orderingType.ConversionPair.Key > 0 && orderingType.ConversionPair.Value != ""; 
        }

        protected abstract int FindKey(string value);

        protected abstract string FindValue(int key);

        protected int MaxKey
        {
            get;
        }

        private int Generation
        {
            get;
        }
    }
}