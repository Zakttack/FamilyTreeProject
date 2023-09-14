using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.OrderingType
{
    public abstract class AbstractOrderingType : IComparable<AbstractOrderingType>
    {
        protected AbstractOrderingType(int key)
        {
            ConversionPair = new(key, FindValue(key));
        }

        protected AbstractOrderingType(string value)
        {
            ConversionPair = new(FindKey(value), value);
        }

        public KeyValuePair<int,string> ConversionPair
        {
            get;
        }

        public int CompareTo(AbstractOrderingType other)
        {
            int tempResult = Type.CompareTo(other.Type);
            return tempResult == 0 ? ConversionPair.Key - other.ConversionPair.Key : tempResult;
        }

        protected abstract OrderingTypeTypes Type
        {
            get;
        }

        protected abstract int FindKey(string value);

        protected abstract string FindValue(int key);
    }
}