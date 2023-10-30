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
    }
}