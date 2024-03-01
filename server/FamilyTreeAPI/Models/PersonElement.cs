using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeAPI.Models
{
    public class PersonElement
    {
        public string Name
        {
            get;
            set;
        }

        public string BirthDate
        {
            get;
            set;
        }

        public string DeceasedDate
        {
            get;
            set;
        }

        public override string ToString()
        {
            string nameMessage = "Name = " + (Name is null ? "unkown" : Name);
            string birthDateMessage = "Birth Date = " + (BirthDate is null ? "unkown" : BirthDate);
            string deceasedDateMessage = "Deceased Date = " + (DeceasedDate is null ? "unkown" : DeceasedDate);
            return $"{nameMessage}; {birthDateMessage}; {deceasedDateMessage};";
        }
    }
}