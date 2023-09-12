using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.OrderingType
{
    public abstract class AbstractOrderingType
    {
        protected AbstractOrderingType(int key, OrderingTypeOptions option = OrderingTypeOptions.None)
        {
            ConversionPair = new(key, FindValue(key));
            Option = option;
        }

        protected AbstractOrderingType(string value, OrderingTypeOptions option = OrderingTypeOptions.None)
        {
            ConversionPair = new(FindKey(value), value);
            Option = option;
        }

        public KeyValuePair<int,string> ConversionPair
        {
            get;
        }

        protected OrderingTypeOptions Option
        {
            get;
        }

        protected abstract int FindKey(string value);

        protected abstract string FindValue(int key);


    }
}