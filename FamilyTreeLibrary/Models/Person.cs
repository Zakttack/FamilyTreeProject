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

        public bool PersonEquivalent(Person other, string familyTreeName)
        {
            bool birthDateCompareWithDefault = FamilyTreeUtils.DateComp.Compare(BirthDate, FamilyTreeUtils.DefaultDate) == 0;
            bool otherBirthDateCompareWithDefault = FamilyTreeUtils.DateComp.Compare(other.BirthDate, FamilyTreeUtils.DefaultDate) == 0;
            bool temp = (birthDateCompareWithDefault && !otherBirthDateCompareWithDefault) || (!birthDateCompareWithDefault && otherBirthDateCompareWithDefault);
            IList<string> parts1 = Name.Split(' ');
            IList<string> parts2 = other.Name.Split(' ');
            bool nameEquivalent = parts1[0] == parts2[0] && parts1.Contains(familyTreeName) && parts2.Contains(familyTreeName);
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
    }
}