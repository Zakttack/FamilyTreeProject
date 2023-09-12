using FamilyTreeLibrary.Exceptions;

namespace FamilyTreeLibrary.OrderingType
{
    public class DigitOrderingType : AbstractOrderingType
    {
        public DigitOrderingType(int key)
        :base(key)
        {

        }

        public DigitOrderingType(string value)
        :base(value)
        {

        }

        protected override int FindKey(string value)
        {
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
            if (key > 0)
            {
                return $"{key}.";
            }
            throw new InvalidKeyException(key);
        }
    }
}