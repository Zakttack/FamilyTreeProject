using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;

namespace FamilyTreeLibrary.PDF.Models
{
    public class Section
    {
        public Section(AbstractOrderingType[] orderingType, Family node)
        {
            OrderingType = orderingType;
            Node = node;
        }
        public AbstractOrderingType[] OrderingType
        {
            get;
        }

        public Family Node
        {
            get;
        }

        public override bool Equals(object obj)
        {
            return obj != null && ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            string orderingTypeOutput = "{" + string.Join<AbstractOrderingType>(',', OrderingType) + "}";
            return $"\nOrderingType: {orderingTypeOutput}\nNode: {Node}";
        }
    }
}