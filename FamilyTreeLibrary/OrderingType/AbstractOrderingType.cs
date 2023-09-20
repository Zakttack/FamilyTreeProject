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

        public static AbstractOrderingType GetOrderingType(int key, int generation)
        {
            return generation switch
            {
                1 => new RomanNumeralOrderingType(key, true),
                2 => new LetterOrderingType(key, true),
                3 => new DigitOrderingType(key, false),
                4 => new LetterOrderingType(key, false),
                5 => new ParenthesizedDigitOrderingType(key),
                6 => new RomanNumeralOrderingType(key, false),
                _ => throw new NotSupportedException($"{generation} is not supported.")
            };
        }

        public static AbstractOrderingType GetOrderingType(string value, int generation)
        {
            return generation switch
            {
                1 => new RomanNumeralOrderingType(value, true),
                2 => new LetterOrderingType(value, true),
                3 => new DigitOrderingType(value, false),
                4 => new LetterOrderingType(value, false),
                5 => new ParenthesizedDigitOrderingType(value),
                6 => new RomanNumeralOrderingType(value, false),
                _ => throw new NotSupportedException($"{generation} is not supported.")
            };
        }

        protected abstract int FindKey(string value);

        protected abstract string FindValue(int key);

        private int Generation
        {
            get;
        }
    }
}