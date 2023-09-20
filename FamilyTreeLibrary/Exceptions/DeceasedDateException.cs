using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Exceptions
{
    public class DeceasedDateException : ArgumentException
    {
        public DeceasedDateException(string name, DateTime birthDate, DateTime date)
        :base($"{name} can't be deceased on {date.ToString().Split()[0]}, since they were born on {birthDate.ToString().Split()[0]}")
        {

        }
    }
}