using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.OrderingType
{
    public abstract class AbstractOrderingType : IComparable<AbstractOrderingType>
    {
        protected AbstractOrderingType(int key, int generation)
        {
            Generation = generation;
            ConversionPair = new(key, FindValue(key));
        }

        protected AbstractOrderingType(string value, int generation)
        {
            Generation = generation;
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
            return Type.CompareTo(other.Type);
        }

        public static bool TryGetOrderingType(out AbstractOrderingType orderingType, int key, int generation)
        {
            switch (generation)
            {
                case 1: orderingType = new RomanNumeralOrderingType(key, true); return true;
                case 2: orderingType = new LetterOrderingType(key, true); return true;
                case 3: orderingType = new DigitOrderingType(key, false); return true;
                case 4: orderingType = new LetterOrderingType(key, false); return true;
                case 5: orderingType = new ParenthesizedDigitOrderingType(key); return true;
                case 6: orderingType = new RomanNumeralOrderingType(key, false); return true;
                default: orderingType = null; return false; 
            }
        }

        public static bool TryGetOrderingType(out AbstractOrderingType orderingType, string value, int generation)
        {
            switch (generation)
            {
                case 1: orderingType = new RomanNumeralOrderingType(value, true); return true;
                case 2: orderingType = new LetterOrderingType(value, true); return true;
                case 3: orderingType = new DigitOrderingType(value, false); return true;
                case 4: orderingType = new LetterOrderingType(value, false); return true;
                case 5: orderingType = new ParenthesizedDigitOrderingType(value); return true;
                case 6: orderingType = new RomanNumeralOrderingType(value, false); return true;
                default: orderingType = null; return false; 
            }
        }

        protected abstract int FindKey(string value);

        protected abstract string FindValue(int key);

        private int Generation
        {
            get;
        }
    }
}