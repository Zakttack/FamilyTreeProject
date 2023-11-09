using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data
{
    public interface IFamilyTree : IEnumerable<Family>
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

        public void Add(Family node);

        public bool Contains(Family node);

        public int Depth(Family node);

        public IFamilyTree Subtree(Family root);
    }
}