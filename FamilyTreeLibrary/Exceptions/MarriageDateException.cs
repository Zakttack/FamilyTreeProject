using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeLibrary.Exceptions
{
    public class MarriageDateException : ArgumentException
    {
        public MarriageDateException(string name, DateTime marriageDate, DateTime birthDate)
         :base($"{name} can't be married on {marriageDate.ToString().Split()[0]}, since {name} was born on {birthDate.ToString().Split()[0]}")
         {}
    }
}