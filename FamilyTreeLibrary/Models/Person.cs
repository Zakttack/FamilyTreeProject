using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyTreeLibrary.Models
{
    public class Person : IComparable<Person>, IEquatable<Person>
    {
        private FamilyTreeDate deceasedDate;
        public Person(string name)
        {
            Name = name;
        }

        [BsonElement(nameof(Name))]
        [BsonDefaultValue(null)]
        [BsonIgnoreIfDefault(false)]
        public string Name
        {
            get;
        }
        [BsonElement(nameof(BirthDate))]
        [BsonSerializer(typeof(DateSerializer))]
        [BsonDefaultValue(null)]
        [BsonIgnoreIfDefault(false)]
        public FamilyTreeDate BirthDate
        {
            get;
            set;
        }
        [BsonElement(nameof(DeceasedDate))]
        [BsonSerializer(typeof(DateSerializer))]
        [BsonDefaultValue(null)]
        [BsonIgnoreIfDefault(false)]
        public FamilyTreeDate DeceasedDate
        {
            get
            {
                return deceasedDate;
            }
            set
            {
                if (value.CompareTo(FamilyTreeDate.DefaultDate) != 0 && value.CompareTo(BirthDate) < 0)
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
            return Name.Split(' ').Union(other.Name.Split(' ')).Count() > 1 ? 0 : Name.CompareTo(other.Name);
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
            return this.ToJson();
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