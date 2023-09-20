using FamilyTreeLibrary.Exceptions;

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
                throw new NotSupportedException("The conversion is only supported for the Numbering Ordering Type.");
            }
            string v = value[..(value.Length - 1)];
            try
            {
                return Convert.ToInt32(v);
            }
            catch (FormatException)
            {
                throw new InvalidValueException(value);
            }
        }

        protected override string FindValue(int key)
        {
            if (Type != OrderingTypeTypes.Numbering)
            {
                throw new NotSupportedException("The conversion is only supported for the Numbering Ordering Type.");
            }
            else if (key > 0)
            {
                return $"{key}.";
            }
            throw new InvalidKeyException(key);
        }
    }
}