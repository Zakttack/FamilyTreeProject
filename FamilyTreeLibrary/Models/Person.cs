using FamilyTreeLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Person
    {
        private FamilyTreeDate deceasedDate;
        public Person(string name)
        {
            Name = name;
        }

        public Person(JObject obj)
        {
            Name = obj[nameof(Name)] == null ? "" : JsonConvert.DeserializeObject<string>(obj[nameof(Name)].ToString());
            BirthDate = obj[nameof(BirthDate)] == null ? new() : new(JsonConvert.DeserializeObject<string>(obj[nameof(BirthDate)].ToString()));
            DeceasedDate = obj[nameof(DeceasedDate)] == null ? new() : new(JsonConvert.DeserializeObject<string>(obj[nameof(DeceasedDate)].ToString()));
        }

        public string Name
        {
            get;
        }

        public FamilyTreeDate BirthDate
        {
            get;
            set;
        }

        public FamilyTreeDate DeceasedDate
        {
            get
            {
                return deceasedDate;
            }
            set
            {
                if (value.CompareTo(new()) != 0 && value.CompareTo(BirthDate) < 0)
                {
                    throw new DeceasedDateException(this, value);
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
                {nameof(Name), Name == null ? JValue.CreateNull() : Name},
                {nameof(BirthDate), BirthDate == default | BirthDate.ToString() == "" ? JValue.CreateNull() : BirthDate.ToString()},
                {nameof(DeceasedDate), DeceasedDate == default | DeceasedDate.ToString() == "" ? JValue.CreateNull() : DeceasedDate.ToString()}
            };
            return obj.ToString();
        }
    }
}