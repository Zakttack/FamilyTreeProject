using FamilyTreeLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Person
    {
        private DateTime deceasedDate;
        public Person(string name)
        {
            Name = name;
            BirthDate = default;
            DeceasedDate = default;
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