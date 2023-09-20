
using FamilyTreeLibrary.Exceptions;

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
            if (Type != OrderingTypeTypes.ParenthesizedNumbering)
            {
                throw new NotSupportedException("This conversion is only supported for the parenthesized digit ordering type.");
            }
            string v = value[1..(value.Length - 2)];
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
            if (Type != OrderingTypeTypes.ParenthesizedNumbering)
            {
                throw new NotSupportedException("This conversion is only supported for the parenthesized digit ordering type.");
            }
            else if (key <= 0)
            {
                throw new InvalidKeyException(key);
            }
            return $"({key})";
        }
    }
}