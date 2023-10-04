using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FamilyTreeLibrary.Models
{
    public class Family
    {
        public Family(Person member, Person inLaw = null, DateTime marriageDate = new())
        {
            Member = member;
            InLaw = inLaw;
            MarriageDate = marriageDate;
            LoadChildren();
        }

        public Family(JObject obj)
        {
            Member = obj["Member"] == null ? null : new(JObject.Parse(obj["Member"].ToString()));
            InLaw = obj["InLaw"] == null ? null : new(JObject.Parse(obj["InLaw"].ToString()));
            MarriageDate = obj["MarriageDate"] == null ? FamilyTreeUtils.DefaultDate : Convert.ToDateTime(JsonConvert.DeserializeObject<string>(obj["MarriageDate"].ToString()));
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

        public Person Member
        {
            get;
        }

        public Person InLaw
        {
            get;
        }

        public ICollection<Family> Children
        {
            private set;
            get;
        }

        public DateTime MarriageDate
        {
            get;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Family)
            {
                return false;
            }
            Family other = (Family)obj;
            return Member == other.Member && InLaw == other.InLaw && FamilyTreeUtils.DateComp.Compare(MarriageDate, other.MarriageDate) == 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            JObject obj = new()
            {
                { "Member", JObject.Parse(Member.ToString()) },
                { "InLaw", JObject.Parse(InLaw.ToString()) },
                {"MarriageDate", MarriageDate.ToString().Split()[0]}
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
            Children = new SortedSet<Family>(FamilyTreeUtils.FamilyComparer);
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