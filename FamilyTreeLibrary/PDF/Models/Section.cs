using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLibrary.Models;
using FamilyTreeLibrary.OrderingType;

namespace FamilyTreeLibrary.PDF.Models
{
    public class Section : IComparable<Section>
    {
        public Section(AbstractOrderingType[] orderingType, Family node, int index)
        {
            OrderingType = orderingType;
            Node = node;
            Index = index;
        }
        public AbstractOrderingType[] OrderingType
        {
            get;
        }

        public Family Node
        {
            get;
        }

        public int Index
        {
            get;
        }

        public int CompareTo(Section section)
        {
            return Index - section.Index;
        }
    }
}