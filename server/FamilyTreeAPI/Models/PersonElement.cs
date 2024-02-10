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
    }
}