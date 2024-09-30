using System.Text.Json.Serialization;
using FamilyTreeLibrary.Data.Comparers;
using FamilyTreeLibrary.Exceptions;
using FamilyTreeLibrary.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyTreeLibrary.Models
{
    public class Person : ICloneable, IComparable<Person>, IEquatable<Person>
    {
        private FamilyTreeDate deceasedDate;

        public Person(string name, FamilyTreeDate birthDate, FamilyTreeDate deceasedDate)
        {
            Name = name;
            BirthDate = birthDate;
            DeceasedDate = deceasedDate;
        }

        public Person(string representation)
        {
            if (representation is null || representation == "")
            {
                Name = null;
                BirthDate = Constants.DefaultDate;
                DeceasedDate = Constants.DefaultDate;
            }
            else
            {
                string[] delimiters = {" (", " - ", ")"};
                string[] parts = representation.Split(delimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                Name = parts[0];
                BirthDate = parts.Length == 3 ? new FamilyTreeDate(parts[1]) : Constants.DefaultDate;
                DeceasedDate = parts.Length == 3 && parts[2] != "Present" ? new FamilyTreeDate(parts[2]) : Constants.DefaultDate;
            }
        }
        public string Name
        {
            get;
        }
        [BsonSerializer(typeof(FamilyTreeDateSerializer))]
        [JsonConverter(typeof(FamilyTreeDateSerializer))]
        public FamilyTreeDate BirthDate
        {
            get;
            set;
        }
        [BsonSerializer(typeof(FamilyTreeDateSerializer))]
        [JsonConverter(typeof(FamilyTreeDateSerializer))]
        public FamilyTreeDate DeceasedDate
        {
            get
            {
                return deceasedDate;
            }
            set
            {
                if (value.CompareTo(Constants.DefaultDate) != 0 && value.CompareTo(BirthDate) < 0)
                {
                    throw new DeceasedDateException(this, value);
                }
                deceasedDate = value;
            }
        }

        public object Clone()
        {
            return new Person(ToString());
        }

        public int CompareTo(Person other)
        {
            if (other is null)
            {
                return 1;
            }
            else if (BirthDate < other.BirthDate)
            {
                return -1;
            }
            else if (BirthDate > other.BirthDate)
            {
                return 1;
            }
            else if (DeceasedDate < other.DeceasedDate)
            {
                return -1;
            }
            else if (DeceasedDate > other.DeceasedDate)
            {
                return 1;
            }
            IComparer<string> nameCompare = new NameComparer();
            return nameCompare.Compare(Name, other.Name);
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
            return HashCode.Combine(BirthDate, DeceasedDate, Name);
        }

        public override string ToString()
        {
            if (Name is not null && BirthDate > Constants.DefaultDate && DeceasedDate > BirthDate)
            {
                return $"{Name} ({BirthDate} - {DeceasedDate})";
            }
            else if (Name is not null && BirthDate > Constants.DefaultDate)
            {
                return $"{Name} ({BirthDate} - Present)";
            }
            else if (Name is not null)
            {
                return Name;
            }
            return null;
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

        public static bool operator< (Person a, Person b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && !bIsNull) || (!aIsNull && a.CompareTo(b) < 0);
        }

        public static bool operator> (Person a, Person b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (!aIsNull && bIsNull) || (!aIsNull && a.CompareTo(b) > 0);
        }
    }
}