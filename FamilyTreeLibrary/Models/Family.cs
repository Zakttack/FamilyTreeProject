using FamilyTreeLibrary.Comparers;
using FamilyTreeLibrary.Exceptions;
using MongoDB.Bson;

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

        public Family(BsonDocument document)
        {
            Id = document.GetValue("Id").AsObjectId;
            BsonValue parentValue = document["Parent"];
            Parent = parentValue.IsBsonNull ? null : new(document["Parent"].AsBsonDocument);
            Member = new(document["Member"].AsBsonDocument);
            BsonValue inLawValue = document["InLaw"];
            InLaw = inLawValue.IsBsonNull ? null : new(document["InLaw"].AsBsonDocument);
            BsonValue marriageDateValue = document["MarriageDate"];
            MarriageDate = marriageDateValue.IsBsonNull ? FamilyTreeDate.DefaultDate : new(document.GetValue("MarriageDate").AsString);
            BsonArray array = document["Children"].AsBsonArray;
            Children = new SortedSet<Person>();
            foreach (BsonValue child in array.Values)
            {
                Children.Add(new(child.AsBsonDocument));
            }
        }
        public ObjectId Id
        {
            get;
        }
        public Person Parent
        {
            get;
            set;
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
        public ICollection<Person> Children
        {
            get;
        }

        public BsonDocument Document
        {
            get
            {
                Dictionary<string,object> doc = new()
                {
                    {"Id", Id},
                    {"Parent", Parent is null ? BsonNull.Value : Parent.Document},
                    {"Member", Member.Document},
                    {"InLaw", InLaw is null ? BsonNull.Value : InLaw.Document},
                    {"MarriageDate", MarriageDate == FamilyTreeDate.DefaultDate ? BsonNull.Value : MarriageDate.ToString()}
                };
                BsonArray array = new();
                foreach (Person child in Children)
                {
                    BsonArray temp = array;
                    array = temp.Add(child.Document);
                }
                doc.Add("Children", array);
                return new(doc);
            }
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
            return Document.ToJson();
        }
    }
}