using FamilyTreeLibrary.Exceptions;
using MongoDB.Bson;

namespace FamilyTreeLibrary.Models
{
    public class Person : IComparable<Person>, IEquatable<Person>
    {
        private FamilyTreeDate deceasedDate;

        public Person(string name, FamilyTreeDate birthDate, FamilyTreeDate deceasedDate)
        {
            Name = name;
            BirthDate = birthDate;
            DeceasedDate = deceasedDate;
        }

        public Person(BsonDocument document)
        {
            Name = document[nameof(Name)].IsBsonNull ? null : document[nameof(Name)].AsString;
            BirthDate = document[nameof(BirthDate)].IsBsonNull ? FamilyTreeDate.DefaultDate : new(document[nameof(BirthDate)].AsString);
            DeceasedDate = document[nameof(DeceasedDate)].IsBsonNull ? FamilyTreeDate.DefaultDate : new(document[nameof(DeceasedDate)].AsString);
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
                if (value.CompareTo(FamilyTreeDate.DefaultDate) != 0 && value.CompareTo(BirthDate) < 0)
                {
                    throw new DeceasedDateException(this, value);
                }
                deceasedDate = value;
            }
        }

        public BsonDocument Document
        {
            get
            {
                Dictionary<string,object> doc = new()
                {
                    {nameof(Name), Name is null ? BsonNull.Value : Name},
                    {nameof(BirthDate), BirthDate == default || BirthDate == FamilyTreeDate.DefaultDate ? 
                        BsonNull.Value : BirthDate.ToString()},
                    {nameof(DeceasedDate), DeceasedDate == default || DeceasedDate == FamilyTreeDate.DefaultDate ? 
                        BsonNull.Value : DeceasedDate.ToString()}
                };
                return new(doc);
            }
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
            bool thisNameIsNull = Name is null;
            bool otherNameIsNull = other.Name is null;
            if (thisNameIsNull && !otherNameIsNull)
            {
                return -1;
            }
            else if (thisNameIsNull && otherNameIsNull)
            {
                return 0;
            }
            else if (!thisNameIsNull && otherNameIsNull)
            {
                return 1;
            }
            string[] nameParts = Name.Split(' ');
            string[] otherNameParts = other.Name.Split(' ');
            return nameParts.Intersect(otherNameParts).Count() >= 2 ? 0 : Name.CompareTo(other.Name);
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
            return BirthDate.GetHashCode() + DeceasedDate.GetHashCode() + Name.GetHashCode();
        }

        public override string ToString()
        {
            return Document.ToJson();
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