using FamilyTreeLibrary.Comparers;
using FamilyTreeLibrary.Data;
using FamilyTreeLibrary.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace FamilyTreeLibrary.Models
{
    public class Family : IComparable<Family>, IEquatable<Family>
    {
        private FamilyTreeDate marriageDate;
        public Family(Person member, ObjectId id = default)
        {
            Id = id == default ? ObjectId.GenerateNewId() : id;
            Member = member;
            Children = new SortedSet<Person>();
        }

        [BsonId]
        public ObjectId Id
        {
            get;
        }
        [BsonElement(nameof(Parent))]
        [BsonDefaultValue(null)]
        [BsonIgnoreIfDefault(false)]
        public Person Parent
        {
            get;
            set;
        }
        [BsonElement(nameof(Member))]
        [BsonDefaultValue(null)]
        [BsonIgnoreIfDefault(false)]
        public Person Member
        {
            get;
            set;
        }
        [BsonElement(nameof(InLaw))]
        [BsonDefaultValue(null)]
        [BsonIgnoreIfDefault(false)]
        public Person InLaw
        {
            get;
            set;
        }
        [BsonElement(nameof(MarriageDate))]
        [BsonSerializer(typeof(DateSerializer))]
        [BsonDefaultValue(null)]
        [BsonIgnoreIfDefault(false)]
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
        [BsonElement(nameof(Children))]
        public ICollection<Person> Children
        {
            get;
        }

        public int CompareTo(Family other)
        {
            if (other is null)
            {
                return 1;
            }
            else if (Parent < other.Parent)
            {
                return -1;
            }
            else if (Parent > other.Parent)
            {
                return 1;
            }
            int memberCompare = Member.CompareTo(other.Member);
            if (memberCompare != 0)
            {
                return memberCompare;
            }
            int marriageCompare = MarriageDate.CompareTo(other.MarriageDate);
            if (marriageCompare != 0)
            {
                return marriageCompare;
            }
            IComparer<IEnumerable<Person>> childrenCompare = new ChildrenComparer();
            return childrenCompare.Compare(Children, other.Children);
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
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }
}