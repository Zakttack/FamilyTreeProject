
using FamilyTreeLibrary.Exceptions;

namespace FamilyTreeLibrary.OrderingType
{
    public class ParenthesizedDigitOrderingType : DigitOrderingType
    {
        public ParenthesizedDigitOrderingType(int key)
        :base(key)
        {

        }

        public ParenthesizedDigitOrderingType(string value)
        :base(value)
        {

        }

        protected override int FindKey(string value)
        {
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
            if (key <= 0)
            {
                throw new InvalidKeyException(key);
            }
            return $"({key})";
        }
    }
}