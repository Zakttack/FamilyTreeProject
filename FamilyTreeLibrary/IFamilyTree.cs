using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary
{
    public interface IFamilyTree : IEnumerable<Person>
    {

        public string Name
        {
            get;
        }

        public Person this[Person member]
        {
            get;
        }

        public void ReportChild(Person parent, Person child);

        public void ReportDeceased(Person p);

        public void ReportMarriage(Person member, Person inLaw);
    }
}