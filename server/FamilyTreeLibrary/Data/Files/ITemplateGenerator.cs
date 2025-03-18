using FamilyTreeLibrary.Data.Models;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary.Data.Files
{
    public interface ITemplateGenerator
    {
        public IReadOnlyDictionary<HierarchialCoordinate, FamilyTreeNode> Family
        {
            get;
        }

        public FileStream WriteTemplate();
    }
}