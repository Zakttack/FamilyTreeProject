using System.Text.RegularExpressions;

namespace FamilyTreeLibrary.Data.PDF.OrderingType
{
    public class ParenthesizedDigitOrderingType : DigitOrderingType
    {
        internal ParenthesizedDigitOrderingType(int key, int maxKey = int.MaxValue)
        :base(key, true, maxKey)
        {
        }

        internal ParenthesizedDigitOrderingType(string value, int maxKey = int.MaxValue)
        :base(value, true, maxKey)
        {
        }

        protected override int FindKey(string value)
        {
            string v = Regex.IsMatch(value, "^\\(\\d+\\)$") ? value[1..^1] : "";
            if (Type == OrderingTypeTypes.ParenthesizedNumbering && Regex.IsMatch(v, FamilyTreeUtils.NUMBER_PATTERN))
            {
                int key = Convert.ToInt32(v);
                return key <= MaxKey ? key : 0;
            }
            return 0;
        }

        protected override string FindValue(int key)
        {
            return Type == OrderingTypeTypes.ParenthesizedNumbering && key > 0 && key <= MaxKey ? $"({key})" : "";
        }
    }
}