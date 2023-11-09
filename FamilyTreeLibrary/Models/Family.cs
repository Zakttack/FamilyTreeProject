using FamilyTreeLibrary.Data.JsonConverters;
using FamilyTreeLibrary.Exceptions;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Family : IComparable<Family>, IEquatable<Family>
    {
        private FamilyTreeDate marriageDate;
        public Family(Person member)
        {
            Id = ObjectId.GenerateNewId();
            Parent = null;
            Member = member;
            Children = new SortedSet<Family>();
        }

        public Family(JObject obj)
        {
            Id = obj[nameof(Id)] == null ? ObjectId.Empty : JsonConvert.DeserializeObject<ObjectId>(obj[nameof(Id)].ToString(), new ObjectIdConverter());
            Parent = obj[nameof(Parent)] == null ? null : JsonConvert.DeserializeObject<Family>(obj[nameof(Parent)].ToString(), new ParnetConverter());
            Member = obj[nameof(Member)] == null ? null : new(JObject.Parse(obj[nameof(Member)].ToString()));
            InLaw = obj[nameof(InLaw)] == null ? null : new(JObject.Parse(obj[nameof(InLaw)].ToString()));
            MarriageDate = obj[nameof(MarriageDate)] == null ? new(0) : JsonConvert.DeserializeObject<FamilyTreeDate>(obj[nameof(MarriageDate)].ToString(), new DateConverter());
            Children = obj[nameof(Children)] == null ? new SortedSet<Family>() : JsonConvert.DeserializeObject<ICollection<Family>>(obj[nameof(Children)].ToString(), new ChildrenConverter());
        }
        [JsonProperty(nameof(Id))]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id
        {
            get;
            set;
        }
        [JsonProperty(nameof(Parent))]
        [JsonConverter(typeof(ParnetConverter))]
        public Family Parent
        {
            get;
            set;
        }
        [JsonProperty(nameof(Member))]
        public Person Member
        {
            get;
            set;
        }
        [JsonProperty(nameof(InLaw))]
        public Person InLaw
        {
            get;
            set;
        }
        [JsonProperty(nameof(MarriageDate))]
        [JsonConverter(typeof(DateConverter))]
        public FamilyTreeDate MarriageDate
        {
            get
            {
                return marriageDate;
            }
            set
            {
                if (value.CompareTo(new(0)) != 0 && (value.CompareTo(Member.BirthDate) < 0 || value.CompareTo(InLaw.BirthDate) < 0))
                {
                    throw new MarriageDateException(this, value);
                }
                marriageDate = value;
            }
        }
        [JsonProperty(nameof(Children))]
        [JsonConverter(typeof(ChildrenConverter))]
        public ICollection<Family> Children
        {
            get;
        }

        public int CompareTo(Family other)
        {
            int memberCompare = Member.CompareTo(other.Member);
            return memberCompare != 0 ? memberCompare : MarriageDate.CompareTo(other.MarriageDate);
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
            return JsonConvert.SerializeObject(this);
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
            return (!aIsNull || !bIsNull) && (aIsNull || !a.Equals(b));
        }
    }
}