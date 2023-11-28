using FamilyTreeLibrary.Data.PDF.OrderingType;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.PDF.Models
{
    public class Section
    {
        public Section(AbstractOrderingType[] orderingType, FamilyNode node)
        {
            OrderingType = orderingType;
            Node = node;
        }
        public AbstractOrderingType[] OrderingType
        {
            get;
        }

        public FamilyNode Node
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