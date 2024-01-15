using FamilyTreeLibrary.Exceptions;
using MongoDB.Bson;

namespace FamilyTreeLibrary.Models
{
    public class Family : IComparable<Family>, IEquatable<Family>
    {
        private FamilyTreeDate marriageDate;
        public Family(Person member, Person inLaw, FamilyTreeDate marriageDate)
        {
            Member = member;
            InLaw = inLaw;
            MarriageDate = marriageDate;
        }

        public Family(BsonDocument document)
        {
            Member = document[nameof(Member)].IsBsonNull ? null : new(document[nameof(Member)].AsBsonDocument);
            InLaw = document[nameof(InLaw)].IsBsonNull ? null : new(document[nameof(InLaw)].AsBsonDocument);
            MarriageDate = document[nameof(MarriageDate)].IsBsonNull ? FamilyTreeDate.DefaultDate : new(document[nameof(MarriageDate)].AsString);
        }
        public Person Member
        {
            get;
        }
        public Person InLaw
        {
            get;
            set;
        }
        public FamilyTreeDate MarriageDate
        {
            get
            {
                return marriageDate;
            }
            set
            {
                if (value.CompareTo(FamilyTreeDate.DefaultDate) != 0 && (value.CompareTo(Member.BirthDate) < 0 || value.CompareTo(InLaw.BirthDate) < 0))
                {
                    throw new MarriageDateException(this, value);
                }
                marriageDate = value;
            }
        }

        public BsonDocument Document
        {
            get
            {
                Dictionary<string,object> doc = new()
                {
                    {nameof(Member), Member is null ? BsonNull.Value : Member.Document},
                    {nameof(InLaw), InLaw is null ? BsonNull.Value : InLaw.Document},
                    {nameof(MarriageDate), MarriageDate == default || MarriageDate == FamilyTreeDate.DefaultDate ?
                        BsonNull.Value : MarriageDate.ToString()}
                };
                return new(doc); 
            }
        }

        public int CompareTo(Family other)
        {
            if (other is null)
            {
                return 1;
            }
            else if (Member < other.Member)
            {
                return -1;
            }
            else if (Member > other.Member)
            {
                return 1;
            }
            else if (MarriageDate < other.MarriageDate)
            {
                return -1;
            }
            else if (MarriageDate > other.MarriageDate)
            {
                return 1;
            }
            else if (InLaw < other.InLaw)
            {
                return -1;
            }
            return InLaw > other.InLaw ? 1 : 0;
        }

        public override bool Equals(object obj)
        {
            return obj is Family other && Equals(other);
        }

        public bool Equals(Family other)
        {
            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Member, InLaw, MarriageDate);
        }

        public override string ToString()
        {
            return Document.ToJson();
        }

        public static bool operator== (Family a, Family b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && bIsNull) || (!aIsNull && a.Equals(b));
        }

        public static bool operator!= (Family a, Family b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && !bIsNull) || (!aIsNull && !a.Equals(b));
        }

        public static bool operator< (Family a, Family b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (aIsNull && !bIsNull) || (!aIsNull && a.CompareTo(b) < 0);
        }

        public static bool operator> (Family a, Family b)
        {
            bool aIsNull = a is null;
            bool bIsNull = b is null;
            return (!aIsNull && bIsNull) || (!aIsNull && a.CompareTo(b) > 0);
        }
    }
}