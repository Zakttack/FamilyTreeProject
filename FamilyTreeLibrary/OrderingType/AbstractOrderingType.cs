using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
                _ => null
            };
        }

        public override string ToString()
        {
            return ConversionPair.ToString();
        }

        public static bool TryGetOrderingType(out AbstractOrderingType orderingType, string value, int generation)
        {
            switch (generation)
            {
                case 1: orderingType = new RomanNumeralOrderingType(value, true); break;
                case 2: orderingType = new LetterOrderingType(value, true); break;
                case 3: orderingType = new DigitOrderingType(value, false); break;
                case 4: orderingType = new LetterOrderingType(value, false); break;
                case 5: orderingType = new ParenthesizedDigitOrderingType(value); break;
                case 6: orderingType = new RomanNumeralOrderingType(value, false); break;
                default: orderingType = null; return false; 
            }
            return orderingType.ConversionPair.Key > 0 && orderingType.ConversionPair.Value != ""; 
        }

        protected abstract int FindKey(string value);

        protected abstract string FindValue(int key);

        private int Generation
        {
            get;
        }
    }
}