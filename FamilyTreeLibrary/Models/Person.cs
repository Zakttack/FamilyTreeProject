using FamilyTreeLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public Person(JObject obj)
        {
            Name = obj["Name"] == null ? "" : JsonConvert.DeserializeObject<string>(obj["Name"].ToString());
            BirthDate = obj["BirthDate"] == null ? FamilyTreeUtils.DefaultDate : Convert.ToDateTime(JsonConvert.DeserializeObject<string>(obj["BirthDate"].ToString()));
            DeceasedDate = obj["DeceasedDate"] == null ? FamilyTreeUtils.DefaultDate : Convert.ToDateTime(JsonConvert.DeserializeObject<string>(obj["DeceasedDate"].ToString()));
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
                if (FamilyTreeUtils.DateComp.Compare(value, FamilyTreeUtils.DefaultDate) != 0 &&
                    FamilyTreeUtils.DateComp.Compare(value, BirthDate) < 0)
                {
                    throw new DeceasedDateException(Name, BirthDate, value);
                }
                deceasedDate = value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool PersonEquivalent(Person p1, Person p2)
        {
            bool birthDateCompareWithDefault = FamilyTreeUtils.DateComp.Compare(p1.BirthDate, FamilyTreeUtils.DefaultDate) == 0;
            bool otherBirthDateCompareWithDefault = FamilyTreeUtils.DateComp.Compare(p2.BirthDate, FamilyTreeUtils.DefaultDate) == 0;
            bool temp = (birthDateCompareWithDefault && !otherBirthDateCompareWithDefault) || (!birthDateCompareWithDefault && otherBirthDateCompareWithDefault);
            ISet<string> parts1 = p1.Name.Split(' ').ToHashSet();
            ISet<string> parts2 = p2.Name.Split(' ').ToHashSet();
            ISet<string> intersection = new HashSet<string>();
            intersection.IntersectWith(parts1);
            intersection.IntersectWith(parts2);
            bool nameEquivalent = intersection.Contains(p1.FirstName) && intersection.Count > 1;
            return temp && nameEquivalent;
        }

        public override string ToString()
        {
            JObject obj = new()
            {
                {"Name", Name},
                {"BirthDate", BirthDate.ToString().Split()[0]},
                {"DeceasedDate", DeceasedDate.ToString().Split()[0]}
            };
            return obj.ToString();
        }

        private string FirstName
        {
            get
            {
                return Name.Split(' ')[0];
            }
        }
    }
}