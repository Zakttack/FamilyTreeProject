using FamilyTreeLibrary.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Family : IComparable<Family>
    {
        private DateTime marriageDate;
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
            object parent = obj["Parent"];
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
            Member = obj["Member"] == null ? default : new(JObject.Parse(obj["Member"].ToString()));
            InLaw = obj["InLaw"] == null ? default : new(JObject.Parse(obj["InLaw"].ToString()));
            MarriageDate = obj["MarriageDate"] == null ? default : Convert.ToDateTime(JsonConvert.DeserializeObject<string>(obj["MarriageDate"].ToString()));
            ICollection<Person> people = new List<Person>();
            if (obj["Children"] != null)
            {
                JArray array = JArray.Parse(obj["Children"].ToString());
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

        public DateTime MarriageDate
        {
            get
            {
                return marriageDate;
            }
            set
            {
                if (FamilyTreeUtils.ComparerDate.Compare(value, default) != 0 &&
                    FamilyTreeUtils.ComparerDate.Compare(value, Member.BirthDate) < 0)
                {
                    throw new MarriageDateException(Member.Name, value, Member.BirthDate);
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
            int birthDateCompare = FamilyTreeUtils.ComparerDate.Compare(Member.BirthDate, other.Member.BirthDate);
            return birthDateCompare == 0 ? FamilyTreeUtils.ComparerDate.Compare(MarriageDate, other.MarriageDate) : birthDateCompare;
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
                {"Parent", Parent == default ? JValue.CreateNull() : JObject.Parse(Parent.Member.ToString())},
                { "Member", Member == default ? JValue.CreateNull() : JObject.Parse(Member.ToString()) },
                { "InLaw", InLaw == default ? JValue.CreateNull() : JObject.Parse(InLaw.ToString()) },
                {"MarriageDate", FamilyTreeUtils.ComparerDate.Compare(MarriageDate, default) == 0 ? JValue.CreateNull() : MarriageDate.ToString().Split()[0]}
            };
            JArray array = new();
            foreach (Family child in Children)
            {
                array.Add(JObject.Parse(child.Member.ToString()));
            }
            obj.Add("Children", array);
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