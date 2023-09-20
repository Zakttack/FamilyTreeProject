using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLibrary.Exceptions;

namespace FamilyTreeLibrary.OrderingType
{
    public class RomanNumeralOrderingType : AbstractOrderingType
    {
        internal RomanNumeralOrderingType(int key, bool isUpperCase)
        :base(key, isUpperCase ? 1 : 6)
        {
        }

        internal RomanNumeralOrderingType(string value, bool isUpperCase)
        :base(value, isUpperCase ? 1 : 6)
        {
        }

        protected override int FindKey(string value)
        {
            string v = value[..(value.Length - 1)].ToUpper();
            foreach (char c in v)
            {
                if (!(c == 'M' || c == 'D' || c == 'C' || c == 'L' || c == 'X' || c == 'V' || c == 'I'))
                {
                    throw new InvalidValueException(value);
                }
            }
            int key = 1;
            while (FindValue(key) != value)
            {
                key++;
            }
            return key;
        }

        protected override string FindValue(int key)
        {
            if (key < 0)
            {
                throw new InvalidKeyException(key);
            }
            string result = "";
            int number = key;
            foreach (KeyValuePair<int,string> item in SpecialConversionPairs)
            {
                while (number >= item.Key)
                {
                    result += item.Value;
                    number -= item.Key;
                }
            }
            switch (Type)
            {
                case OrderingTypeTypes.RomanNumeralUpper: result += "."; break;
                case OrderingTypeTypes.RomanNumeralLower: result = result.ToLower() + ")"; break;
                default: throw new ArgumentNullException("Ordering Type Options", "A Roman Numeral is either capitalized or not capitalized.");
            }
            return result;
        }

        private IReadOnlyDictionary<int,string> SpecialConversionPairs
        {
            get
            {
                return new Dictionary<int,string>()
                {
                    { 1000, "M" },
                    { 900, "CM" },
                    { 500, "D" },
                    { 400, "CD" },
                    { 100, "C" },
                    { 90, "XC" },
                    { 50, "L" },
                    { 40, "XL" },
                    { 10, "X" },
                    { 9, "IX" },
                    { 5, "V" },
                    { 4, "IV" },
                    { 1, "I" }
                };
            }
        }
    }
}