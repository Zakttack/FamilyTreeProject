using System.Collections;
using FamilyTreeLibrary.OrderingType;

namespace FamilyTreeLibrary.Models
{
    public class OrderingTypeNode
    {
        public OrderingTypeNode(string content, params string[] args)
        {
            Validate(args);
            AbstractOrderingType[] orderingTypes = new AbstractOrderingType[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                switch (i)
                {
                    case 0: orderingTypes[i] = new RomanNumeralOrderingType(args[0], OrderingTypeOptions.UpperCase); break;
                    case 1: orderingTypes[i] = new LetterOrderingType(args[i], OrderingTypeOptions.UpperCase); break;
                    case 2: orderingTypes[i] = new DigitOrderingType(args[i]); break;
                    case 3: orderingTypes[i] = new LetterOrderingType(args[i], OrderingTypeOptions.LowerCase); break;
                    case 4: orderingTypes[i] = new ParenthesizedDigitOrderingType(args[i]); break;
                    case 5: orderingTypes[i] = new RomanNumeralOrderingType(args[i], OrderingTypeOptions.LowerCase); break;
                }
            }
            Node = new(orderingTypes, content);
        }

        public KeyValuePair<AbstractOrderingType[],string> Node
        {
            get;
        }

        private void Validate(params string[] args)
        {
            if (args.Length < 1 && args.Length > 6)
            {
                throw new NotSupportedException("Only generations 1-6 are supported.");
            }
        }
    }
}