using FamilyTreeLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Person : IComparable<Person>
    {
        private DateTime deceasedDate;
        public Person(string name, DateTime birthDate, DateTime deceasedDate = default)
        {
            Name = name;
            BirthDate = birthDate;
            DeceasedDate = deceasedDate;
        }

        public Person(JObject obj)
        {
            Name = obj["Name"] == null ? "" : JsonConvert.DeserializeObject<string>(obj["Name"].ToString());
            BirthDate = obj["BirthDate"] == null ? default : Convert.ToDateTime(JsonConvert.DeserializeObject<string>(obj["BirthDate"].ToString()));
            DeceasedDate = obj["DeceasedDate"] == null ? default : Convert.ToDateTime(JsonConvert.DeserializeObject<string>(obj["DeceasedDate"].ToString()));
        }

        public string Name
        {
            get;
        }

        public DateTime BirthDate
        {
            get;
            set;
        }

        public DateTime DeceasedDate
        {
            get
            {
                return deceasedDate;
            }
            set
            {
                if (FamilyTreeUtils.ComparerDate.Compare(value, default) != 0 &&
                    FamilyTreeUtils.ComparerDate.Compare(value, BirthDate) < 0)
                {
                    throw new DeceasedDateException(Name, BirthDate, value);
                }
                deceasedDate = value;
            }
        }

        public int CompareTo(Person person)
        {
            int birthDateCompare = FamilyTreeUtils.ComparerDate.Compare(BirthDate, person.BirthDate);
            return birthDateCompare == 0 ? Name.CompareTo(person.Name) : birthDateCompare;
        }

        public override bool Equals(object obj)
        {
            return obj != null && ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            JObject obj = new()
            {
                {"Name", Name == null ? JValue.CreateNull() : Name},
                {"BirthDate", FamilyTreeUtils.ComparerDate.Compare(BirthDate, default) == 0 ? JValue.CreateNull() : BirthDate.ToString().Split()[0]},
                {"DeceasedDate", FamilyTreeUtils.ComparerDate.Compare(DeceasedDate, default) == 0 ? JValue.CreateNull() : DeceasedDate.ToString().Split()[0]}
            };
            return obj.ToString();
        }
    }
}