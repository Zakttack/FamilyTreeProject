using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data
{
    public interface ITree : IEnumerable<FamilyNode> 
    {
        public long Count
        {
            get;
        }

        public IEnumerable<Family> this[Person member]
        {
            get;
        }

        public int Height
        {
            get;
        }

        public string Name
        {
            get;
        }

        public FamilyNode Root
        {
            get;
        }

        public void Add(FamilyNode node);

        public bool Contains(FamilyNode node);

        public int Depth(FamilyNode node);

        public IEnumerable<FamilyNode> GetChildren(FamilyNode node);

        public FamilyNode GetParent(FamilyNode node);
        public ITree Subtree(FamilyNode root);

        public void Update(FamilyNode initialNode, FamilyNode finalNode);
    }
}