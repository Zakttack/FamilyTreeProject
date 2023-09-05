using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyTreeLibrary.Models;

namespace FamilyTreeLibrary
{
    public interface IFamilyTree : IEnumerable<Person>
    {
        public int Count
        {
            get;
        }

        public string Name
        {
            get;
        }

        public void AddFamily(Person parent, Person child);

        public Person GetInLaw(Person member);

        public void UpdateMarriage(Person member);
    }
}