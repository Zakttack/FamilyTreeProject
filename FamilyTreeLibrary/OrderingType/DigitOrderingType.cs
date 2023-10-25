using FamilyTreeLibrary.Exceptions;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary.OrderingType
{
    public class DigitOrderingType : AbstractOrderingType
    {
        internal DigitOrderingType(int key, bool isParenthesized)
        :base(key, !isParenthesized ? 3 : 5)
        {
        }

        internal DigitOrderingType(string value, bool isParenthesized)
        :base(value, !isParenthesized ? 3 : 5)
        {
        }

        protected override int FindKey(string value)
        {
            if (Type != OrderingTypeTypes.Numbering)
            {
                return 0;
            }
            string v = value.Length > 0 ? value[..(value.Length - 1)] : value;
            return Regex.IsMatch(v, FamilyTreeUtils.NUMBER_PATTERN) ? Convert.ToInt32(v) : 0;
        }

        protected override string FindValue(int key)
        {
            return Type == OrderingTypeTypes.Numbering && key > 0 ? $"{key}." : "";
        }
    }
}