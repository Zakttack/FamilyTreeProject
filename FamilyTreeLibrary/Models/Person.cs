using FamilyTreeLibrary.Exceptions;
using Newtonsoft.Json;

namespace FamilyTreeLibrary.Models
{
    public class Person
    {
        private DateTime deceasedDate;
        public Person(string name, DateTime birthDate, DateTime deceasedDate = new())
        {
            Name = name;
            BirthDate = birthDate;
            DeceasedDate = deceasedDate;
        }

        public string Name
        {
            get;
        }

        public DateTime BirthDate
        {
            get;
        }

        public DateTime DeceasedDate
        {
            get
            {
                return deceasedDate;
            }
            set
            {
                if (FamilyTreeUtils.CompareDates(value, FamilyTreeUtils.DefaultDate) != 0 &&
                    FamilyTreeUtils.CompareDates(value, BirthDate) < 0)
                {
                    throw new DeceasedDateException(Name, BirthDate, value);
                }
                deceasedDate = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not Person)
            {
                return false;
            }
            Person other = (Person)obj;
            return ToString() == other.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}