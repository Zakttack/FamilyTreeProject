using FamilyTreeLibrary.Exceptions;
using MongoDB.Bson;

namespace FamilyTreeLibrary.Models
{
    public class Person : IComparable<Person>, IEquatable<Person>
    {
        private FamilyTreeDate deceasedDate;

        public Person(string name)
        {
            Name = name;
        }

        public Person(BsonDocument document)
        {
            Name = document.GetValue("Name").AsString;
            BirthDate = new(document.GetValue("BirthDate").AsString);
            DeceasedDate = new(document.GetValue("DeceasedDate").AsString);
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
                    {"Name", Name},
                    {"BirthDate", BirthDate.ToString()},
                    {"DeceasedDate", DeceasedDate.ToString()}
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
            return Name.CompareTo(other.Name);
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