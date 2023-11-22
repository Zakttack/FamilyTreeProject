using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data
{
    public interface IFamilyTree : IEnumerable<Family>
    {
        public long Count
        {
            get;
        }

        public Family this[Person member]
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

        public void Add(Family parentNode, Family childNode);

        public bool Contains(Family node);

        public int Depth(Family node);

        public IFamilyTree Subtree(Family root);

        public void Update(Family initialNode, Family finalNode);
    }
}