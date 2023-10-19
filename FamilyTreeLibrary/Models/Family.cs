using FamilyTreeLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Family : IComparable<Family>
    {
        private FamilyTreeDate marriageDate;
        public Family(Person member)
        {
            Parent = default;
            Member = member;
            InLaw = default;
            MarriageDate = default;
            LoadChildren();
        }

        public Family(JObject obj)
        {
            object parent = obj[nameof(Parent)];
            if (parent == default)
            {
                Parent = default;
            }
            else
            {
                JObject parentMember = JObject.Parse(parent.ToString());
                Person member = new(parentMember);
                Parent = new(member);
            }
            Member = obj[nameof(Member)] == null ? default : new(JObject.Parse(obj[nameof(Member)].ToString()));
            InLaw = obj[nameof(InLaw)] == null ? default : new(JObject.Parse(obj[nameof(InLaw)].ToString()));
            MarriageDate = obj[nameof(MarriageDate)] == null ? new() : new(JsonConvert.DeserializeObject<string>(obj[nameof(MarriageDate)].ToString()));
            ICollection<Person> people = new List<Person>();
            if (obj[nameof(Children)] != null)
            {
                JArray array = JArray.Parse(obj[nameof(Children)].ToString());
                foreach (JToken token in array)
                {
                    if (token != null)
                    {
                        people.Add(new(JObject.Parse(token.ToString())));
                    }
                }
            }
            LoadChildren(people);
        }

        public Family Parent
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
                if (value.CompareTo(new()) != 0 && (value.CompareTo(Member.BirthDate) < 0 || value.CompareTo(InLaw.BirthDate) < 0))
                {
                    throw new MarriageDateException(this, value);
                }
                marriageDate = value;
            }
        }

        public ICollection<Family> Children
        {
            private set;
            get;
        }

        public int CompareTo(Family other)
        {
            int birthDateCompare = Member.BirthDate.CompareTo(other.Member.BirthDate);
            return birthDateCompare == 0 ? MarriageDate.CompareTo(other.MarriageDate) : birthDateCompare;
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
                {nameof(Parent), Parent == default ? JValue.CreateNull() : JObject.Parse(Parent.Member.ToString())},
                {nameof(Member), Member == default ? JValue.CreateNull() : JObject.Parse(Member.ToString()) },
                {nameof(InLaw), InLaw == default ? JValue.CreateNull() : JObject.Parse(InLaw.ToString()) },
                {nameof(MarriageDate), MarriageDate.CompareTo(new()) == 0 ? JValue.CreateNull() : MarriageDate.ToString()}
            };
            JArray array = new();
            foreach (Family child in Children)
            {
                array.Add(JObject.Parse(child.Member.ToString()));
            }
            obj.Add(nameof(Children), array);
            return obj.ToString();
        }

        private void LoadChildren(IEnumerable<Person> people = null)
        {
            Children = new SortedSet<Family>();
            if (people != null)
            {
                foreach (Person child in people)
                {
                    Children.Add(new(child));
                }
            }
        }
    }
}