using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Exceptions
{
    public class DeceasedDateException : ArgumentException
    {
        public DeceasedDateException(string name, DateTime birthDate, DateTime date)
        :base($"{name} can't be deceased on {FamilyTreeUtils.WriteDate(date)}, since they were born on {FamilyTreeUtils.WriteDate(birthDate)}")
        {

        }
    }
}