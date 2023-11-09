using FamilyTreeLibrary.Data.JsonConverters;
using FamilyTreeLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Person : IComparable<Person>, IEquatable<Person>
    {
        private FamilyTreeDate deceasedDate;
        public Person(string name)
        {
            Name = name;
        }

        public Person(JObject obj)
        {
            Name = obj[nameof(Name)] == null ? "" : JsonConvert.DeserializeObject<string>(obj[nameof(Name)].ToString());
            BirthDate = obj[nameof(BirthDate)] == null ? new(0) : JsonConvert.DeserializeObject<FamilyTreeDate>(obj[nameof(BirthDate)].ToString(), new DateConverter());
            DeceasedDate = obj[nameof(DeceasedDate)] == null ? new(0) : JsonConvert.DeserializeObject<FamilyTreeDate>(obj[nameof(DeceasedDate)].ToString(), new DateConverter());
        }

        [JsonProperty(nameof(Name))]
        public string Name
        {
            get;
        }
        [JsonProperty(nameof(BirthDate))]
        [JsonConverter(typeof(DateConverter))]
        public FamilyTreeDate BirthDate
        {
            get;
            set;
        }
        [JsonProperty(nameof(DeceasedDate))]
        [JsonConverter(typeof(DateConverter))]
        public FamilyTreeDate DeceasedDate
        {
            get
            {
                return deceasedDate;
            }
            set
            {
                if (value.CompareTo(new(0)) != 0 && value.CompareTo(BirthDate) < 0)
                {
                    throw new DeceasedDateException(this, value);
                }
                deceasedDate = value;
            }
        }

        public int CompareTo(Person other)
        {
            int birthDateCompare = BirthDate.CompareTo(other.BirthDate);
            if (birthDateCompare != 0)
            {
                return birthDateCompare;
            }
            int deceasedDateCompare = DeceasedDate.CompareTo(deceasedDate);
            if (deceasedDateCompare != 0)
            {
                return deceasedDateCompare;
            }
            int nameCompare = Name.CompareTo(other.Name);
            return Name.Split(' ').Union(other.Name.Split(' ')).Count() > 1 ? 0 : nameCompare;
        }

        public override bool Equals(object obj)
        {
            return obj is Person other && Equals(other);
        }

        public bool Equals(Person other)
        {
            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static bool operator== (Person a, Person b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && bIsNull) || (!aIsNull && a.Equals(b)); 
        }

        public static bool operator!= (Person a, Person b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (!aIsNull || !bIsNull) && (aIsNull || !a.Equals(b));
        }
    }
}