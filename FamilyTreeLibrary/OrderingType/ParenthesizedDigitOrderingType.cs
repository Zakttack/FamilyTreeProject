
using FamilyTreeLibrary.Exceptions;
using System.Text.RegularExpressions;

namespace FamilyTreeLibrary.OrderingType
{
    public class ParenthesizedDigitOrderingType : DigitOrderingType
    {
        internal ParenthesizedDigitOrderingType(int key)
        :base(key, true)
        {
        }

        internal ParenthesizedDigitOrderingType(string value)
        :base(value, true)
        {
        }

        protected override int FindKey(string value)
        {
            string v = Regex.IsMatch(value, "^\\(\\d+\\)$") ? value[1..^1] : "";
            return Type == OrderingTypeTypes.ParenthesizedNumbering && Regex.IsMatch(v, FamilyTreeUtils.NUMBER_PATTERN) ? Convert.ToInt32(v) : 0;
        }

        protected override string FindValue(int key)
        {
            return Type == OrderingTypeTypes.ParenthesizedNumbering && key > 0 ? $"({key})" : "";
        }
    }
}